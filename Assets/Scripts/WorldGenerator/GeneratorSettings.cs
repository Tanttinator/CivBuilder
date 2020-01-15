using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorSettings : MonoBehaviour {

    public WorldGenerator.GenerationMode generationMode;
    public float scale = 20f;
    [Range(0, 8)]
    public int octaves = 4;
    [Range(0, 1)]
    public float persistance = 0.5f;
    public float lacunarity = 2;
    [Range(0, 10)]
    public float continentLandAmount = 2.2f;
    public float meshHeightMultiplier = 15f;
    public AnimationCurve meshHeightCurve;
    public bool flatShading = false;

    [Tooltip("To make autoupdating work, make sure to have a function called \"GenerateMap\" on the same object as this class and that class has the attribute \"ExecuteInEditMode\" ")]
    public bool autoUpdate;

    public static GeneratorSettings singleton;

    private void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
    }

}
