using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CivInfoPanelUI : MonoBehaviour {

    public Text nameText;
    public Text population;

    public void RegisterCallbacks()
    {
        Civilization.RegisterOnNameSet(SetName);
        Worker.RegisterOnPopulationChanged(PopulationChanged);
    }

    private void OnDisable()
    {
        Civilization.UnregisterOnNameSet(SetName);
        Worker.UnregisterOnPopulationChanged(PopulationChanged);
    }

    public void PopulationChanged(int unemployed, int total, int max)
    {
        population.text = "Population: " + total;
    }

    void SetName(string name)
    {
        nameText.text = name;
    }
}
