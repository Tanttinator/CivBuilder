using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public static class Settings {

    //Audio
    public static float volume;

    //Video
    public static Resolution resolution;

    public static FullScreenMode fullscreen;

    public static void Apply()
    {
        Screen.SetResolution(resolution.width, resolution.height, fullscreen, resolution.refreshRate);
        /*PlayerSettings.defaultScreenWidth = resolution.width;
        PlayerSettings.defaultScreenHeight = resolution.height;
        PlayerSettings.fullScreenMode = fullscreen;*/
        SoundHandler.instance.source.volume = volume;

        Save();
    }

    public static void Load()
    {
        if (PlayerPrefs.HasKey("Volume"))
            volume = Mathf.Clamp01(PlayerPrefs.GetFloat("Volume"));
        else
            volume = 0.5f;

        /*fullscreen = PlayerSettings.fullScreenMode;
        resolution = new Resolution();
        resolution.width = PlayerSettings.defaultScreenWidth;
        resolution.height = PlayerSettings.defaultScreenHeight;*/

        Apply();
    }

    public static void Save()
    {
        PlayerPrefs.SetFloat("Volume", volume);
    }

}
