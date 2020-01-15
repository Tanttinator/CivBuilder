using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityTabUI : MonoBehaviour {

    public Text nameText;
    public Text populationText;
    public Text happinessText;
    public Text loyaltyText;

	public void Setup(City city)
    {
        nameText.text = city.name;
        SetPopulation(city.unemployed, city.population, city.maxPopulation);
        SetHappiness(city.happiness);
        SetLoyalty(city.loyalty);
        city.RegisterOnPopulationChanged(SetPopulation);
        city.RegisterOnHappinessChanged(SetHappiness);
        city.RegisterOnLoyaltyChanged(SetLoyalty);
    }

    void SetPopulation(int unemployed, int total, int max)
    {
        populationText.text = unemployed + "/" + total + "/" + max;
    }

    void SetHappiness(float happiness)
    {
        happinessText.text = Mathf.RoundToInt(happiness * 100) + "%";
    }

    void SetLoyalty(float loyalty)
    {
        loyaltyText.text = Mathf.RoundToInt(loyalty * 100) + "%";
    }
}
