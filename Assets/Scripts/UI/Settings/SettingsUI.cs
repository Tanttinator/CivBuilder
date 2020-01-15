using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour {

    public Dropdown resolutions;
    public Slider volume;

    private void Awake()
    {
        List<Dropdown.OptionData> data = new List<Dropdown.OptionData>();
        foreach(Resolution res in Screen.resolutions)
        {
            data.Add(new Dropdown.OptionData(res.ToString()));
        }
        resolutions.onValueChanged.AddListener(delegate { Settings.resolution = Screen.resolutions[resolutions.value]; });

        volume.value = Settings.volume;
        volume.onValueChanged.AddListener(delegate { Settings.volume = volume.value; });
    }

    public void Apply()
    {
        Settings.Apply();
    }
}
