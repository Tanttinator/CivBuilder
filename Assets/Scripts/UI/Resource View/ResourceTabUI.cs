using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResourceTabUI : MonoBehaviour, IPointerClickHandler
{
    public Transform details;
    public GameObject cityDetailTab;
    Dictionary<City, GameObject> cityDetailTabs = new Dictionary<City, GameObject>();
    float height;

    public Text nameText;
    public Text amountText;
    public Text incomeText;

    LayoutElement le;
    LayoutElement detailsLe;

    Resource resource;

    bool isOpen = false;

    public void Setup(Resource resource)
    {
        this.resource = resource;
        nameText.text = resource.name;
        amountText.text = ResourcePool.inventory.AmountOf(resource).ToString();
        UpdateIncome();
        GameController.RegisterOnGameTick(UpdateIncome);
        ResourcePool.RegisterOnResourceSet(SetAmount);
        le = GetComponent<LayoutElement>();
        detailsLe = details.GetComponent<LayoutElement>();
        height = cityDetailTab.GetComponent<RectTransform>().rect.height;
        City.RegisterOnCityCreated(AddCity);
        foreach (City city in City.cities)
            AddCity(city);
        Hide();
    }

    private void OnDisable()
    {
        ResourcePool.UnregisterOnResourceSet(SetAmount);
        GameController.UnregisterOnGameTick(UpdateIncome);
    }

    void SetAmount(Resource resource, int amount)
    {
        if(resource == this.resource)
            amountText.text = "" + amount;
    }

    float GetIncome()
    {
        float income = 0f;

        foreach (Building building in Building.buildings)
            income += building.GetIncome(resource);

        return income;
    }

    void UpdateIncome()
    {
        float income = (float)System.Math.Round(GetIncome(), 2);
        incomeText.text = "<color=" + (income < 0 ? "red" : "green") + ">" + income + "</color>";
    }

    void AddCity(City city)
    {
        GameObject tab = Instantiate(cityDetailTab, details);
        tab.GetComponent<CityDetailTabUI>().Setup(city);
        cityDetailTabs.Add(city, tab);
        if (isOpen)
        {
            le.preferredHeight += height;
            detailsLe.preferredHeight += height;
        }
    }

    void Show()
    {
        le.preferredHeight = 40f + height * cityDetailTabs.Count;
        detailsLe.preferredHeight = height * cityDetailTabs.Count;
        details.localScale = new Vector3(1, 1, 1);
    }

    void Hide()
    {
        le.preferredHeight = 40f;
        detailsLe.preferredHeight = 0f;
        details.localScale = new Vector3(0, 0, 0);
    }

    void Toggle()
    {
        isOpen = !isOpen;

        if (isOpen)
            Show();
        else
            Hide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Toggle();
    }
}
