using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingHandler : MonoBehaviour {

    [Header("Building Ghost")]
    [HideInInspector] public bool isPlacingBuilding = false;
    [HideInInspector] public bool isOverlapping = false;
    bool validPosition = false;
    public GameObject buildingGhost;
    Projector ghostRangeDisplay;
    Projector ghostDirDisplay;
    [HideInInspector] public Building placedBuilding;
    Dictionary<BuildingClass, Building> closestOfTypes = new Dictionary<BuildingClass, Building>();
    List<BuildingClass> showRangeTypes = new List<BuildingClass>();
    
    public GameObject rangeDisplay;
    public Material roadMat;
    public float roadWidth;

    [HideInInspector()] public int population = 0;

    public static BuildingHandler instance;

    public void StartPlaceBuilding(Building building)
    {
        GameObject prefab = building.prefab;
        isPlacingBuilding = true;
        InputHandler.escActionStack.Add(CancelBuild);
        placedBuilding = building;

        MeshFilter[] meshFilters = prefab.GetComponentsInChildren<MeshFilter>(true);
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = Instantiate(meshFilters[i].sharedMesh);
            combine[i].transform = Matrix4x4.TRS(meshFilters[i].transform.position, meshFilters[i].transform.rotation, meshFilters[i].transform.localScale);
        }
        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        mesh.RecalculateBounds();
        BoxCollider prefabCollider = prefab.GetComponent<BoxCollider>();
        if(prefabCollider == null)
        {
            Debug.LogError(prefab.name + " is missing a box collider!");
            return;
        }
        buildingGhost.GetComponent<MeshFilter>().mesh = Instantiate(mesh);
        buildingGhost.GetComponent<BoxCollider>().center = prefabCollider.center;
        buildingGhost.GetComponent<BoxCollider>().size = prefabCollider.size;

        showRangeTypes = Building.GetShowRangeTypes(building.buildingClass);

        if(building.range > 0)
        {
            ghostRangeDisplay.orthographicSize = building.range * World.ScaleFactor;
            ghostRangeDisplay.enabled = true;
        }
        ghostDirDisplay.transform.position = buildingGhost.transform.TransformPoint(new Vector3(0, 0, buildingGhost.GetComponent<BoxCollider>().size.z));

        buildingGhost.transform.rotation = Quaternion.Euler(prefab.transform.rotation.eulerAngles.x, buildingGhost.transform.rotation.eulerAngles.y - buildingGhost.transform.rotation.eulerAngles.y % 15f, 0f);
        buildingGhost.SetActive(true);
    }

    public void PlaceBuilding()
    {
        if (placedBuilding != null && ((validPosition && ResourcePool.inventory.HasResources(placedBuilding.cost)) || GameController.debug))
        {
            RaycastHit hit = InputHandler.GetMousePos();
            Building.CreateBuilding(placedBuilding, hit.point, Quaternion.LookRotation(new Vector3(buildingGhost.transform.forward.x, 0f, buildingGhost.transform.forward.z)));
        }
        CancelBuild();
    }

    public void PlaceBuildingGO(Building building)
    {
        GameObject buildingGO = Instantiate(building.prefab, building.location, building.rotation, transform);
        buildingGO.transform.localScale *= World.ScaleFactor;
        buildingGO.layer = LayerMask.NameToLayer("Building");
        if(building.range > 0)
        {
            Projector rangeDisplayGO = Instantiate(rangeDisplay, buildingGO.transform).GetComponent<Projector>();
            rangeDisplayGO.transform.localPosition = new Vector3(0f, 1f, 0f);
            rangeDisplayGO.enabled = false;
            rangeDisplayGO.orthographicSize = building.range;
            building.rangeDisplay = rangeDisplayGO;
        }
        //FIXME: check if over multiple chunks
        if (building.flatten)
            World.GetChunkAtWorldPoint(building.location).Flatten(building.location.y, buildingGO.GetComponent<BoxCollider>().bounds);
        Building.buildingGOMap.Add(buildingGO, building);
        building.GO = buildingGO;
    }

    public void CancelBuild()
    {
        isOverlapping = false;
        isPlacingBuilding = false;
        placedBuilding = null;
        ghostRangeDisplay.enabled = false;
        buildingGhost.SetActive(false);
        UIHandler.HideTooltip();
        foreach (Building building in closestOfTypes.Values)
        {
            if(building != null)
                building.HideRange();
        }
        closestOfTypes.Clear();
        showRangeTypes.Clear();
        InputHandler.escActionStack.Remove(CancelBuild);
    }

    public bool CheckValidity(RaycastHit hit)
    {
        if (GameController.debug)
            return true;
        if (isOverlapping)
        {
            UIHandler.DisplayTooltip("Overlapping with another building!");
            return false;
        }
        float angle = 90f - Vector3.Angle(hit.normal, new Vector3(hit.normal.x, 0, hit.normal.z));
        if (angle > placedBuilding.maxAngle)
        {
            UIHandler.DisplayTooltip("Too Steep!");
            return false;
        }
        if(angle < placedBuilding.minAngle)
        {
            UIHandler.DisplayTooltip("Too Flat!");
            return false;
        }
        if (!(placedBuilding is Central) && !BuildingHandler.ConstructionEnabled(hit.point))
        {
            UIHandler.DisplayTooltip("Requires central building! (campfire)");
            return false;
        }
        float height = World.GetHeight(buildingGhost.transform.position);
        if(height < 0)
        {
            UIHandler.DisplayTooltip("Position is invalid. The Dev probably fucked up.");
            return false;
        }
        switch (placedBuilding.placeTerrain)
        {
            case Building.PlaceTerrain.Land:
                if (height < 0.401)
                {
                    UIHandler.DisplayTooltip("Requires land!");
                    return false;
                }
                break;
            case Building.PlaceTerrain.Coast:
                if(height < 0.4 || height > 0.405)
                {
                    UIHandler.DisplayTooltip("Requires coast!");
                    return false;
                }
                break;
            case Building.PlaceTerrain.Water:
                if(height > 0.4)
                {
                    UIHandler.DisplayTooltip("Requires water!");
                    return false;
                }
                break;
        }
        if (!placedBuilding.CanBuild())
        {
            return false;
        }
        UIHandler.HideTooltip();
        return true;
    }

    public void RotateGhostRight()
    {
        if (isPlacingBuilding)
        {
            buildingGhost.transform.Rotate(new Vector3(0, 15f, 0), Space.World);
        }
    }

    public void RotateGhostLeft()
    {
        if (isPlacingBuilding)
        {
            buildingGhost.transform.Rotate(new Vector3(0, -15f, 0), Space.World);
        }
    }

    public static bool ConstructionEnabled(Vector3 location)
    {
        foreach(Building building in Building.buildings)
        {
            if (building is Central)
            {
                if(Vector3.Distance(location, building.location) < building.range)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void BuildRoad(Vector2 start, Vector2 end)
    {
        World.GetPath(start, end, OnRoadPathGenerated);
    }

    void OnRoadPathGenerated(Queue<Node> path)
    {
        if (path == null || path.Count < 2)
            return;
        List<Vector3[]> verts = new List<Vector3[]>();
        Vector2? prevPos = null;
        Vector2 position;
        Vector2 nextPos;

        Vector2 a;
        Vector2 b;

        while (path.Count > 0)
        {
            position = path.Dequeue().position;
            if (path.Count > 0)
            {
                nextPos = path.Peek().position;
                b = nextPos - position;
            }
            else
            {
                b = position - (Vector2)prevPos;
            }

            if(prevPos != null)
            {
                a = position - (Vector2)prevPos;
            }
            else
            {
                a = b;
            }

            Vector3[] points = new Vector3[2];
            //0 = right, 1 = left

            Vector2 right;
            Vector2 left;
            float width = roadWidth;

            if(a == b)
            {
                left = Vector2.Perpendicular(a);
                right = -left;
            }
            else
            {
                if(Vector2.SignedAngle(a, b) > 0)
                {
                    right = a - b;
                    left = b - a;
                    width = roadWidth / Mathf.Sin(Vector2.Angle(a, right));
                }
                else
                {
                    right = b - a;
                    left = a - b;
                    width = roadWidth / Mathf.Sin(Vector2.Angle(a, left));
                }
            }

            right = right.normalized * (width / 2);
            left = left.normalized * (width / 2);

            points[0] = World.GetPointAt(position + right).point + new Vector3(0, 0.1f, 0);
            points[1] = World.GetPointAt(position + left).point + new Vector3(0, 0.1f, 0);

            verts.Add(points);

            prevPos = position;
        }
        GameObject roadGO = new GameObject("Road");
        roadGO.transform.SetParent(transform);
        for(int i = 0; i < verts.Count - 1; i++)
        {
            GameObject roadStep = new GameObject("Step " + i);
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[4];
            Vector2[] uvs = new Vector2[4];
            int[] tris = new int[6];

            Vector3[] curr = verts[i];
            Vector3[] next = verts[i + 1];

            vertices[0] = curr[1];
            vertices[1] = curr[0];
            vertices[2] = next[1];
            vertices[3] = next[0];

            uvs[0] = new Vector2(0, 0);
            uvs[1] = new Vector2(1, 0);
            uvs[2] = new Vector2(0, 1);
            uvs[3] = new Vector2(1, 1);

            tris[0] = 0;
            tris[1] = 3;
            tris[2] = 1;
            tris[3] = 3;
            tris[4] = 0;
            tris[5] = 2;

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = tris;
            mesh.RecalculateNormals();

            roadStep.AddComponent<MeshFilter>().mesh = mesh;
            roadStep.AddComponent<MeshRenderer>().material = roadMat;
            roadStep.transform.SetParent(roadGO.transform);
        }
    }



    private void Awake()
    {
        instance = this;
        //GameController.RegisterOnGameStarted(OnGameStart);
    }

    void Start()
    {
        buildingGhost.SetActive(false);
        buildingGhost.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        buildingGhost.transform.localScale *= World.ScaleFactor;
        ghostRangeDisplay = buildingGhost.GetComponentsInChildren<Projector>()[0];
        ghostRangeDisplay.enabled = false;
        ghostDirDisplay = buildingGhost.GetComponentsInChildren<Projector>()[1];
    }

    private void Update()
    {
        if (isPlacingBuilding)
        {
            foreach(BuildingClass type in showRangeTypes)
            {
                if(!closestOfTypes.ContainsKey(type) || closestOfTypes[type] != Building.GetClosestOfTypes(buildingGhost.transform.position, type))
                {
                    if (closestOfTypes.ContainsKey(type))
                        closestOfTypes[type].HideRange();
                    else
                        closestOfTypes.Add(type, Building.GetClosestOfTypes(buildingGhost.transform.position, type));

                    closestOfTypes[type] = Building.GetClosestOfTypes(buildingGhost.transform.position, type);
                    if(closestOfTypes[type] != null)
                        closestOfTypes[type].DisplayRange();

                }
            }
            if (!InputHandler.IsMouseOverTerrain())
                return;
            RaycastHit hit = InputHandler.GetMousePos();
            buildingGhost.transform.position = hit.point;

            if (placedBuilding.alignWithTerrain)
            {
                buildingGhost.transform.rotation = Quaternion.LookRotation(new Vector3(hit.normal.x, 0, hit.normal.z));
            }

            if (!validPosition)
            {
                if (CheckValidity(hit))
                {
                    validPosition = true;
                    buildingGhost.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 1f, 1f, 0.7f));
                }
            }
            else
            {
                if (!CheckValidity(hit))
                {
                    validPosition = false;
                    buildingGhost.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 0f, 0f, 0.7f));
                }
            }
        }
    }
}

