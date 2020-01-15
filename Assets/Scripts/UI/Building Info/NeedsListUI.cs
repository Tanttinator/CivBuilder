using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeedsListUI : MonoBehaviour {

    public Text happinessText;
    public GameObject consumableTab;
    Residential building;
    List<GameObject> consumableTabs = new List<GameObject>();

    void AddConsumable(Consumable consumable)
    {
        GameObject go = Instantiate(consumableTab, GetComponent<BuildingInfoListUI>().content);
        go.GetComponent<ConsumableTabUI>().Setup(consumable);
        consumableTabs.Add(go);
    }

    public void Setup(Residential building)
    {
        foreach (GameObject go in consumableTabs)
            Destroy(go);
        consumableTabs.Clear();
        this.building = building;
        foreach (Consumable consumable in building.myConsumables)
            AddConsumable(consumable);
    }

    private void Update()
    {
        if(building != null)
            happinessText.text = building.happiness + "%";
    }
}
