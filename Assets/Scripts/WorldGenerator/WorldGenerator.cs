using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldGenerator {

    public enum GenerationMode
    {
        Normal,
        Continent
    }
    
	public static float[,] GenerateHeightmap(int width, int height, int seed, Vector2 offset, GeneratorSettings settings)
    {
        float scale = settings.scale;
        int octaves = settings.octaves;
        float persistance = settings.persistance;
        float lacunarity = settings.lacunarity;
        float continentLandAmount = settings.continentLandAmount;
        GenerationMode mode = settings.generationMode;

        float[,] heightmap = new float[width, height];
        float[,] falloffMap = GenerateFalloffMap(width, height, continentLandAmount);


        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1f;
        float frequency = 1f;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if(scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxHeight = float.MinValue;
        float minHeight = float.MaxValue;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {

                amplitude = 1f;
                frequency = 1f;
                float heightValue = 0f;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    heightValue += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if(mode == GenerationMode.Continent)
                {
                    heightValue = Mathf.Clamp01(heightValue - falloffMap[x, y]);
                }

                if(heightValue > maxHeight)
                {
                    maxHeight = heightValue;
                }else if(heightValue < minHeight)
                {
                    minHeight = heightValue;
                }
                heightmap[x, y] = heightValue;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                /*float heightValue = Mathf.InverseLerp(minHeight, maxHeight, heightmap[x, y]);
                heightmap[x, y] = heightValue;*/

                float normalizedHeight = (heightmap[x, y] + 1) / (2f * maxPossibleHeight / 2.1f);
                heightmap[x, y] = normalizedHeight;
            }
        }

        return heightmap;
    }

    public static float[,] GenerateHumiditymap(int width, int height, int seed, Vector2 offset, GeneratorSettings settings)
    {
        float scale = settings.scale;
        int octaves = settings.octaves;
        float persistance = settings.persistance;
        float lacunarity = settings.lacunarity;

        float[,] heightmap = new float[width, height];


        System.Random prng = new System.Random(seed + 10);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1f;
        float frequency = 1f;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxHeight = float.MinValue;
        float minHeight = float.MaxValue;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                amplitude = 1f;
                frequency = 1f;
                float heightValue = 0f;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    heightValue += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (heightValue > maxHeight)
                {
                    maxHeight = heightValue;
                }
                else if (heightValue < minHeight)
                {
                    minHeight = heightValue;
                }
                heightmap[x, y] = heightValue;
            }
        }

        //Debug.Log(maxPossibleHeight + ", smallest value: " + minHeight + ", largest value: "+ maxHeight);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                /*float heightValue = Mathf.InverseLerp(minHeight, maxHeight, heightmap[x, y]);
                heightmap[x, y] = heightValue;*/
                
                float normalizedHeight = (heightmap[x, y] + 1.3f) / (2f * maxPossibleHeight / 1.47f);
                heightmap[x, y] = normalizedHeight;
            }
        }

        return heightmap;
    }

    public static float[,] GenerateFalloffMap(int sizeX, int sizeY, float landAmount)
    {
        float[,] falloffMap = new float[sizeX, sizeY];

        for(int x = 0; x < sizeX; x++)
        {
            for(int y = 0; y < sizeY; y++)
            {
                float valueX = x / (float)sizeX * 2 - 1;
                float valueY = y / (float)sizeY * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(valueX), Mathf.Abs(valueY));
                falloffMap[x, y] = Evaluate(value, landAmount);
            }
        }

        return falloffMap;
    }

    static float Evaluate(float value, float b)
    {
        float a = 3f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }

    public static Color[] GenerateColorMap(float[,] heightmap, List<TerrainLayer> layers)
    {
        int width = heightmap.GetLength(0);
        int height = heightmap.GetLength(1);
        Color[] colors = new Color[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    if (heightmap[x, y] < layers[i].maxHeight)
                    {
                        colors[x + y * width] = layers[i].color;
                        break;
                    }
                }
            }
        }

        return colors;
    }

    public static Color[] GenerateColorMap(float[,] heightmap, float[,] humiditymap, List<TerrainLayer> layers)
    {
        int width = heightmap.GetLength(0);
        int height = heightmap.GetLength(1);
        Color[] colors = new Color[width * height];

        bool validHeight;
        bool validHumidity;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                foreach (TerrainLayer layer in layers)
                {
                    validHeight = false;
                    validHumidity = false;

                    if(layer.minHeight < heightmap[x, y] && layer.maxHeight > heightmap[x, y])
                    {
                        validHeight = true;
                    }
                    if (layer.minHumidity < humiditymap[x, y] && layer.maxHumidity > humiditymap[x, y])
                    {
                        validHumidity = true;
                    }

                    if(validHeight && validHumidity)
                    {
                        colors[x + y * width] = layer.color;
                        break;
                    }

                }
            }
        }

        return colors;
    }

    public static Texture2D GenerateTexture(float[,] heightmap, List<TerrainLayer> layers)
    {
        int width = heightmap.GetLength(0);
        int height = heightmap.GetLength(1);
        Color[] colors = GenerateColorMap(heightmap, layers);

        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(colors);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }

    public static Texture2D GenerateTexture(float[,] heightmap, float[,] humiditymap, List<TerrainLayer> layers)
    {
        int width = heightmap.GetLength(0);
        int height = heightmap.GetLength(1);
        Color[] colors = GenerateColorMap(heightmap, humiditymap, layers);

        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(colors);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }

    public static Texture2D GenerateTexture(Color[] colors, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(colors);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }
}

[System.Serializable]
public struct TerrainLayer
{
    public float minHeight;
    public float maxHeight;

    public float minHumidity;
    public float maxHumidity;

    public Color color;
}

