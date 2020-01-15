using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityViewUI : MonoBehaviour {

    public GameObject cityTab;
    public Transform cityList;
    public Dictionary<City, GameObject> cityTabs = new Dictionary<City, GameObject>();

    public void PreStart()
    {
        foreach (GameObject go in cityTabs.Values)
            Destroy(go);
        cityTabs.Clear();
    }

    public void RegisterCallbacks()
    {
        City.RegisterOnCityCreated(AddCity);
    }

    private void OnDisable()
    {
        City.UnregisterOnCityCreated(AddCity);
    }

    void AddCity(City city)
    {
        GameObject tab = Instantiate(cityTab, cityList);
        tab.GetComponent<CityTabUI>().Setup(city);
        cityTabs.Add(city, tab);
    }
}
