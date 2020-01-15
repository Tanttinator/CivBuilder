using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Threading;

public class World : MonoBehaviour {

    [Header("Map settings")]
    [Range(0.1f, 1f)]
    public float humiditySize;
    public static int forestDensity = 2;
    public Vector2 offset;
    public List<TerrainLayer> layers;
    public int startingChunkCount = 3;
    static GeneratorSettings settings;
    public static int chunkSize = 256;

    public static int seed { get; protected set; }
    public static void SetSeed(int seed)
    {
        World.seed = seed;
        TerrainChunk.prng = new System.Random(seed + 1);
    }

    public static float ScaleFactor
    {
        get
        {
            return settings.scale / 275f;
        }
    }

    static Dictionary<Vector2, Node> graph;
    static int edgeCount = 0;
    static Queue<PathThreadInfo> pathCallbackQueue = new Queue<PathThreadInfo>();

    public static Dictionary<string, GameObject> terrainObjects = new Dictionary<string, GameObject>();

    public Material chunkMat;
    public static Dictionary<Coord, TerrainChunk> chunks;
    public static int claimedChunks = 0;

    Queue<ChunkThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<ChunkThreadInfo<MapData>>();

    static Action<Coord> onChunkGenerated;

    public static World instance;

    public void Init()
    {
        if (instance == null)
        {
            instance = this;
            GameController.RegisterOnMenuStarted(OnMenuStarted);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        settings = GetComponent<GeneratorSettings>();
        graph = new Dictionary<Vector2, Node>();
        DontDestroyOnLoad(gameObject);
        chunks = new Dictionary<Coord, TerrainChunk>();
        foreach (GameObject go in Resources.LoadAll<GameObject>("Terrain"))
        {
            go.layer = LayerMask.NameToLayer("TerrainObject");
            go.tag = "TerrainObject";
            terrainObjects.Add(go.name, go);
        }
    }

    private void Update()
    {
        if(mapDataThreadInfoQueue.Count > 0)
        {
            for(int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                ChunkThreadInfo<MapData> chunkThreadInfo = mapDataThreadInfoQueue.Dequeue();
                chunkThreadInfo.callback(chunkThreadInfo.data, chunkThreadInfo.chunk, chunkThreadInfo.generateEnemies, chunkThreadInfo.onChunkGenerated);
            }
        }

        if(pathCallbackQueue.Count > 0)
        {
            PathThreadInfo info = pathCallbackQueue.Dequeue();
            info.callback(info.path);
        }
    }

    void OnMenuStarted()
    {
        GenerateMenuBackgorund();
    }

    #region Chunk Generation

    public void GenerateWorld(GameData.WorldData data)
    {
        claimedChunks = 0;
        ClearChunks();
        SetSeed(data.seed);
        foreach(Coord coord in data.generatedChunks)
        {
            if (data.claimedChunks.Contains(coord))
                GenerateChunk(coord, ClaimChunk, false);
            else
                GenerateChunk(coord);
        }
    }

    void ClearChunks()
    {
        while(chunks.Count > 0)
        {
            TerrainChunk chunk = chunks.First<KeyValuePair<Coord, TerrainChunk>>().Value;
            chunks.Remove(chunk.coord);
            if(chunk.meshObject != null && chunk.meshObject.GetComponent<MeshCollider>() != null)
                chunk.meshObject.GetComponent<MeshCollider>().enabled = false;
            Destroy(chunk.meshObject);
        }
        graph.Clear();
        chunks.Clear();
    }

    public void GeneratePreview()
    {
        ClearChunks();
        GenerateChunk(new Coord(0, 0), DisplayChunk, false);
    }

    public void GenerateMenuBackgorund()
    {
        ClearChunks();
        SetSeed(UnityEngine.Random.Range(-999999, 999999));
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                GenerateChunk(new Coord(x, y), DisplayChunk, false);
            }
        }
    }

    public void GenerateChunk(Coord coord, Action<Coord> callback = null, bool generateEnemies = true)
    {
        if (chunks.ContainsKey(coord))
            return;

        float[,] heightmap = WorldGenerator.GenerateHeightmap(chunkSize, chunkSize, seed, coord * (chunkSize - 1), settings);
        float[,] humiditymap = WorldGenerator.GenerateHumiditymap(chunkSize, chunkSize, seed, coord * (chunkSize - 1), settings);
        Texture2D texture = WorldGenerator.GenerateTexture(heightmap, humiditymap, layers);
        MeshData mesh = MeshGenerator.GenerateMesh(heightmap, settings.meshHeightMultiplier, settings.meshHeightCurve, settings.flatShading);
        TerrainChunk chunk = new TerrainChunk(coord, heightmap, humiditymap, texture, mesh.CreateMesh(), generateEnemies);
        chunks.Add(coord, chunk);
        if (callback != null)
            callback(coord);
        if (onChunkGenerated != null)
            onChunkGenerated(coord);
    }

    public void GenerateChunkThreaded(Coord coord, Action<Coord> callback = null, bool generateEnemies = true)
    {
        if (chunks.ContainsKey(coord))
            return;
        ThreadStart threadStart = delegate
        {
            GenerateChunkThread(ChunkGenerated, coord, generateEnemies, callback);
        };

        new Thread(threadStart).Start();
    }

    void GenerateChunkThread(Action<MapData, Coord, bool, Action<Coord>> callback, Coord coord, bool generateEnemies, Action<Coord> onChunkGenerated)
    {
        float[,] heightmap = WorldGenerator.GenerateHeightmap(chunkSize, chunkSize, seed, coord * (chunkSize - 1), settings);
        float[,] humiditymap = WorldGenerator.GenerateHumiditymap(chunkSize, chunkSize, seed, coord * (chunkSize - 1), settings);
        Color[] colorMap = WorldGenerator.GenerateColorMap(heightmap, humiditymap, layers);
        MeshData mesh = MeshGenerator.GenerateMesh(heightmap, settings.meshHeightMultiplier, settings.meshHeightCurve, settings.flatShading);
        MapData mapData = new MapData(heightmap, humiditymap, colorMap, mesh);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new ChunkThreadInfo<MapData>(mapData, callback, coord, generateEnemies, onChunkGenerated));
        }
    }

    void ChunkGenerated(MapData data, Coord coord, bool generateEnemies, Action<Coord> callback)
    {
        TerrainChunk chunk = new TerrainChunk(coord, data.heightmap, data.humiditymap, data.GenerateTexture(), data.mesh.CreateMesh(), generateEnemies);
        chunks.Add(coord, chunk);
        if (callback != null)
            callback(coord);
        if (onChunkGenerated != null)
            onChunkGenerated(coord);
    }

    public bool TryBuyChunk(Coord chunkCoord)
    {
        if (!chunks.ContainsKey(chunkCoord) || chunks[chunkCoord].claimed)
            return false;
        ResourceStack cost = new ResourceStack(Resource.influence, claimedChunks * 100);
        if (!ResourcePool.inventory.HasResources(cost) && !GameController.debug)
            return false;
        if (!GameController.debug)
            ResourcePool.inventory.Remove(cost);

        ClaimChunk(chunkCoord);
        return true;
    }

    public void ClaimChunk(Coord chunkCoord)
    {
        if (!chunks.ContainsKey(chunkCoord) || chunks[chunkCoord].claimed)
            return;
        DisplayChunk(chunkCoord);
        chunks[chunkCoord].claimed = true;
        claimedChunks++;
        GenerateGraphForChunkStart(chunks[chunkCoord]);
    }

    public void DisplayChunk(int x, int y)
    {
        chunks[new Coord(x, y)].DisplayMesh(chunkSize - 1, chunkMat).transform.SetParent(transform);
    }

    public void DisplayChunk(Coord coord)
    {
        chunks[coord].DisplayMesh(chunkSize - 1, chunkMat).transform.SetParent(transform);
    }

    #endregion

    #region Helpers

    public static TerrainChunk GetChunkAtWorldPoint(Vector3 point)
    {
        Coord coord = new Coord(Mathf.FloorToInt(point.x / World.chunkSize), Mathf.FloorToInt(point.z / World.chunkSize));
        if (chunks.ContainsKey(coord))
            return chunks[coord];
        else
            return null;
    }

    public static RaycastHit GetPointAt(Vector2 position)
    {
        RaycastHit hit;
        Physics.Raycast(new Vector3(position.x, 0f, position.y) + new Vector3(0f, 1000f, 0f), Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain"));
        return hit;
    }

    public static float GetHeight(Vector3 point)
    {
        TerrainChunk chunk = GetChunkAtWorldPoint(point);
        if (chunk == null)
            return -1;
        int X = Mathf.FloorToInt(point.x - chunk.coord.x * chunkSize);
        int Y =  Mathf.FloorToInt(point.z - chunk.coord.y * chunkSize);
        if (X < 0 || X > chunkSize - 1 || Y < 0 || Y > chunkSize - 1)
        {
            Debug.Log(X + ", " + Y + "     " + point + "     " + chunk.coord);
            return -1f;
        }
        return chunk.heightmap[X, Y];
    }

    public static float GetHeight(Vector2 point)
    {
        return GetHeight(new Vector3(point.x, 0f, point.y));
    }

    public static float GetHumidity(Vector3 point)
    {
        TerrainChunk chunk = GetChunkAtWorldPoint(point);
        if (chunk == null)
            return -1;
        int X = Mathf.FloorToInt(point.x - chunk.coord.x * chunkSize);
        int Y = Mathf.FloorToInt(point.z - chunk.coord.y * chunkSize);
        if (X < 0 || X > chunkSize - 1 || Y < 0 || Y > chunkSize - 1)
        {
            Debug.Log(X + ", " + Y + "     " + point + "     " + chunk.coord);
            return -1f;
        }
        return chunk.humiditymap[X, Y];
    }

    public static float GetHumidity(Vector2 point)
    {
        return GetHumidity(new Vector3(point.x, 0f, point.y));
    }

    public static Texture2D GetChunkTexture(Coord chunk)
    {
        return WorldGenerator.GenerateTexture(chunks[chunk].heightmap, chunks[chunk].humiditymap, World.instance.layers);
    }

    #endregion

    #region Pathfinding

    public static void GenerateGraphForChunkStart(TerrainChunk chunk)
    {
        ThreadStart thread = delegate
        {
            GenerateGraphForChunk(chunk);
        };
        new Thread(thread).Start();
    }

    public static void GenerateGraphForChunk(TerrainChunk chunk)
    {
        lock (graph)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    Vector2 position = new Vector2(chunk.coord.x * chunkSize + x, chunk.coord.y * chunkSize + y);
                    graph.Add(position, new Node(position));
                }
            }

            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    Vector2 position = new Vector2(chunk.coord.x * chunkSize + x, chunk.coord.y * chunkSize + y);
                    CreateEdges(graph[position], position + new Vector2(0, 1), position + new Vector2(0, -1), position + new Vector2(1, 0), position + new Vector2(-1, 0), position + new Vector2(1, 1), position + new Vector2(1, -1), position + new Vector2(-1, -1), position + new Vector2(-1, 1));
                }
            }

            if (chunks.ContainsKey(new Coord(chunk.coord.x, chunk.coord.y + 1)))
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    Vector2 position = new Vector2(chunk.coord.x * chunkSize + x, chunk.coord.y * chunkSize);
                    CreateEdges(graph[position], new Vector2(position.x, position.y - 1));
                }
            }
            if (chunks.ContainsKey(new Coord(chunk.coord.x, chunk.coord.y - 1)))
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    Vector2 position = new Vector2(chunk.coord.x * chunkSize + x, chunk.coord.y * chunkSize + chunkSize - 1);
                    CreateEdges(graph[position], new Vector2(position.x, position.y + 1));
                }
            }
            if (chunks.ContainsKey(new Coord(chunk.coord.x + 1, chunk.coord.y)))
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    Vector2 position = new Vector2(chunk.coord.x * chunkSize, chunk.coord.y * chunkSize + y);
                    CreateEdges(graph[position], new Vector2(position.x - 1, position.y));
                }
            }
            if (chunks.ContainsKey(new Coord(chunk.coord.x - 1, chunk.coord.y)))
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    Vector2 position = new Vector2(chunk.coord.x * chunkSize + chunkSize - 1, chunk.coord.y * chunkSize + y);
                    CreateEdges(graph[position], new Vector2(position.x + 1, position.y));
                }
            }
        }
        //Debug.Log("Graph generated with " + graph.Count + " nodes and " + edgeCount + " edges.");
    }

    static void CreateEdges(Node from, params Vector2[] targets)
    {
        foreach (Vector2 to in targets)
        {
            if (graph.ContainsKey(to))
            {
                float height = GetHeight(to);
                float cost = Mathf.Infinity;

                if (height < 0.4)
                {
                    cost = Mathf.Infinity;
                }
                else if (height < 0.44)
                {
                    cost = 10f;
                }
                else if (height < 0.9)
                {
                    cost = 1f;
                }
                else
                {
                    cost = 50f;
                }

                if (cost < Mathf.Infinity)
                {
                    cost += Mathf.Abs(Mathf.Pow(height * 10, 3) - Mathf.Pow(GetHeight(from.position) * 10, 3));
                }

                if (cost < 470)
                {
                    from.edges.Add(new Edge(graph[to], cost));
                    edgeCount++;
                }
            }
        }
    }

    public static void GetPath(Vector2 start, Vector2 end, Action<Queue<Node>> callback)
    {

        start = new Vector2(Mathf.RoundToInt(start.x), Mathf.RoundToInt(start.y));
        end = new Vector2(Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y));

        if (GetChunkAtWorldPoint(start) == null || GetChunkAtWorldPoint(end) == null || !graph.ContainsKey(start) || !graph.ContainsKey(end))
            return;

        if (graph[start].edges.Count == 0 || graph[end].edges.Count == 0)
            return;

        AStar.GeneratePathThreaded(new Graph(graph), graph[start], graph[end], callback, pathCallbackQueue);
    }

    #endregion

    #region Saving / Loading

    public GameData.WorldData SaveWorld()
    {
        GameData.WorldData data = new GameData.WorldData();
        data.seed = seed;

        List<Coord> generatedChunks = new List<Coord>();
        List<Coord> claimedChunks = new List<Coord>();

        foreach(Coord chunk in chunks.Keys)
        {
            generatedChunks.Add(chunk);
            if (chunks[chunk].claimed)
                claimedChunks.Add(chunk);
        }

        data.generatedChunks = generatedChunks.ToArray();
        data.claimedChunks = claimedChunks.ToArray();

        return data;
    }

    #endregion

    #region Callbacks

    public static void RegisterOnChunkGenerated(Action<Coord> callback)
    {
        onChunkGenerated += callback;
    }

    public static void UnregisterOnChunkGenerated(Action<Coord> callback)
    {
        onChunkGenerated -= callback;
    }

    #endregion

    public class TerrainChunk
    {
        public Coord coord;
        public GameObject meshObject;
        public Texture2D texture;
        public bool claimed;
        public float[,] heightmap;
        public float[,] humiditymap;
        public List<Resource> minerals = new List<Resource>();
        public Mesh mesh;

        public List<Unit> enemies = new List<Unit>();

        public static System.Random prng;

        Action onEnemiesKilled;

        public TerrainChunk(Coord coord, float[,] heightmap, float[,] humiditymap, Texture2D texture, Mesh mesh, bool generateEnemies)
        {
            this.coord = coord;
            this.heightmap = heightmap;
            this.humiditymap = humiditymap;
            this.texture = texture;
            this.mesh = mesh;
            claimed = false;
            /*string worldSeed = Math.Abs(World.seed).ToString();
            string seed = (World.seed < 0? "-" : "") + Mathf.Abs(coord.x) + "" + worldSeed.Substring(0, (int)(worldSeed.Length / 2)) + "" + (coord.x < 0? 1 : 0) + "" + (coord.y < 0 ? 1 : 0) + "" + worldSeed.Substring((int)(worldSeed.Length / 2), (int)(worldSeed.Length / 2)) + "" + Mathf.Abs(coord.y);
            prng = new System.Random(int.Parse(seed));*/
            
            if(generateEnemies && prng.Next(100) < 50)
            {
                enemies = Unit.GetEnemies(Age.currentAge, 2 + (claimedChunks - 1) * 2);
            }

            minerals.Add(Resource.salt);
            if (prng.Next(100) < 50)
                minerals.Add(Resource.copperOre);
            else
                minerals.Add(Resource.goldOre);
        }

        public void KillEnemies()
        {
            enemies.Clear();
            if (onEnemiesKilled != null)
                onEnemiesKilled();
        }

        public GameObject DisplayMesh(int size, Material mat)
        {
            meshObject = new GameObject("Chunk " + coord);
            meshObject.AddComponent<MeshFilter>().mesh = mesh;
            MeshCollider collider = meshObject.AddComponent<MeshCollider>();
            //collider.sharedMesh.Clear();
            collider.sharedMesh = mesh;
            meshObject.AddComponent<MeshRenderer>().material = mat;
            meshObject.GetComponent<MeshRenderer>().material.mainTexture = texture;
            meshObject.transform.position = new Vector3(coord.x * size, 0, coord.y * size);
            meshObject.layer = LayerMask.NameToLayer("Terrain");

            for(int x = 0; x < size; x += forestDensity)
            {
                for(int y = 0; y < size; y += forestDensity)
                {
                    GameObject prefab = null;
                    Vector3 point = GetPointAt(new Vector2(x + ((float)(prng.NextDouble() * forestDensity) - (forestDensity / 2f)) + coord.x * chunkSize, y + ((float)(prng.NextDouble() * forestDensity / 2f) - (forestDensity / 2f)) + coord.y * chunkSize)).point;
                    if (point == new Vector3(0, 0, 0))
                        continue;

                    if (heightmap[x, y] > 0.44 && heightmap[x, y] < 0.95 && prng.Next(0, 40 * (int)ScaleFactor) == 0)
                    {
                        prefab = terrainObjects["Rock1"];
                    }
                    else if(heightmap[x, y] > 0.44 && heightmap[x, y] < 0.8 && humiditymap[x, y] > 0.4 && prng.Next(0, 5 * (int)ScaleFactor) == 0)
                    {
                        prefab = terrainObjects["Grass"];
                    }
                    else if(heightmap[x, y] < 0.8 && heightmap[x, y] > 0.44 && Mathf.Pow(Mathf.Max(0, (humiditymap[x, y] - 0.5f)) * 10f, 1.5f) > prng.Next(0, 15 * (int)ScaleFactor))
                    {
                        float rng = prng.Next(100) / 100f;
                        if (rng < 0.33)
                            prefab = terrainObjects["Oak1"];
                        else if (rng < 67)
                            prefab = terrainObjects["Spruce1"];
                        else
                            prefab = terrainObjects["Bush1"];
                    }
                    else if(heightmap[x, y] < 0.8 && heightmap[x, y] > 0.44 && humiditymap[x, y] < 0.4 && prng.Next(0, 40 * (int)ScaleFactor) == 0)
                    {
                        prefab = terrainObjects["Cactus1"];
                    }
                    if(prefab != null)
                        Instantiate(prefab, point, Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0), meshObject.transform).transform.localScale *= ScaleFactor;
                }
            }

            return meshObject;
        }

        public void Flatten(float height, Bounds bounds)
        {
            Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            for(int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = vertices[i];
                Vector3 globalPos = new Vector3(meshObject.transform.TransformPoint(v).x, bounds.center.y, meshObject.transform.TransformPoint(v).z);
                if (bounds.Contains(globalPos))
                {
                    vertices[i] = meshObject.transform.InverseTransformPoint(globalPos.x, height, globalPos.z);
                }
            }
            mesh.vertices = vertices;
        }

        public void RegisterOnEnemiesKilled(Action callback)
        {
            onEnemiesKilled += callback;
        }
    }

    struct MapData
    {
        public float[,] heightmap;
        public float[,] humiditymap;
        public Color[] colorMap;
        public MeshData mesh;

        public MapData(float[,] heightmap, float[,] humiditymap, Color[] colorMap, MeshData mesh)
        {
            this.heightmap = heightmap;
            this.humiditymap = humiditymap;
            this.colorMap = colorMap;
            this.mesh = mesh;
        }

        public Texture2D GenerateTexture()
        {
            return WorldGenerator.GenerateTexture(colorMap, heightmap.GetLength(0), heightmap.GetLength(1));
        }
    }

    struct ChunkThreadInfo<T>
    {
        public T data;
        public Action<T, Coord, bool, Action<Coord>> callback;
        public Coord chunk;
        public bool generateEnemies;
        public Action<Coord> onChunkGenerated;

        public ChunkThreadInfo(T data, Action<T, Coord, bool, Action<Coord>> callback, Coord chunk, bool generateEnemies, Action<Coord> onChunkGenerated){
            this.data = data;
            this.callback = callback;
            this.chunk = chunk;
            this.generateEnemies = generateEnemies;
            this.onChunkGenerated = onChunkGenerated;
        }
    }
}

[System.Serializable]
public struct Coord
{
    public int x;
    public int y;

    public Coord(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }

    public static implicit operator Vector2(Coord c)
    {
        return new Vector2(c.x, c.y);
    }

    public static implicit operator Vector3(Coord c)
    {
        return new Vector3(c.x, 0, c.y);
    }

    public static Coord operator *(Coord c, int i)
    {
        return new Coord(c.x * i, c.y * i);
    }

    public static Coord Random(int min, int max)
    {
        return new Coord(UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max));
    }
}
