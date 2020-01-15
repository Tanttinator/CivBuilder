using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PopulationAutobuyUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.isOn = false;
    }

    private void Update()
    {
        if (toggle.isOn && ResourcePool.inventory.HasType(ResourceType.Food, 10) && Worker.population < Worker.maxPopulation)
            Worker.AddPopulation();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIHandler.DisplayTooltip("Autobuy Population\nBuys population when possible");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIHandler.HideTooltip();
    }
}
