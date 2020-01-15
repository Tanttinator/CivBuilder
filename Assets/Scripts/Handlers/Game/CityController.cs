using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityController : MonoBehaviour {

    public GameObject borderDisplay;
    public GameObject cityNamePanel;

    public static CityController instance;

    private void Awake()
    {
        instance = this;
    }

    public void CreateCity(City city, CityCenter center)
    {
        City.cities.Add(city);
        CreateCityNamePanel(city, center.center);
    }

    public void PlaceBorderDisplay(Vector3 position, float range)
    {
        Projector display = Instantiate(borderDisplay, position, Quaternion.Euler(90f, 0f, 0f), transform).GetComponent<Projector>();
        display.orthographicSize = range;
    }

    public void CreateCityNamePanel(City city, Vector3 position)
    {
        Instantiate(cityNamePanel, UIHandler.instance.canvas.transform).GetComponent<CityNameUI>().Setup(city, position);
    }
}
