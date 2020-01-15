using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GeneratorSettings))]
public class GeneratorSettingsEditor : Editor {

    public override void OnInspectorGUI()
    {
        GeneratorSettings settings = (GeneratorSettings)target;
        if (DrawDefaultInspector())
        {
            if (settings.autoUpdate)
            {
                settings.SendMessage("GenerateWorld", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

}
