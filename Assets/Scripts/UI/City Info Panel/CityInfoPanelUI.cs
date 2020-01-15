using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityInfoPanelUI : MonoBehaviour {

    public Text nameText;
    public Text population;
    public Text unemployed;
    public Text max;

    City city;

    private void Awake()
    {
        Hide();
    }

    public void Show(City city)
    {
        if (this.city != null)
            city.UnregisterOnPopulationChanged(PopulationChanged);
        this.city = city;
        nameText.text = city.name;
        city.RegisterOnPopulationChanged(PopulationChanged);
        PopulationChanged(city.unemployed, city.population, city.maxPopulation);
        InputHandler.escActionStack.Add(Hide);
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void Hide()
    {
        transform.localScale = new Vector3(0, 0, 0);
        InputHandler.escActionStack.Remove(Hide);
        if (city != null)
        {
            city.UnregisterOnPopulationChanged(PopulationChanged);
            city = null;
        }
    }

    private void OnDisable()
    {
        if (city != null)
            city.UnregisterOnPopulationChanged(PopulationChanged);
    }

    public void PopulationChanged(int unemployed, int total, int max)
    {
        population.text = "Population: "+ total;
        this.unemployed.text = "Unemployed: " + unemployed;
        this.max.text = "Max Population: " + max;
    }
}
