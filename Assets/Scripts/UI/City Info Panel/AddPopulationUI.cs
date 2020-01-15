using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AddPopulationUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    Button button;
    City city;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Setup(City city)
    {
        this.city = city;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(delegate { city.AddPopulation(); });
    }

    private void Update()
    {
        if (button.interactable)
        {
            if ((city == null || !ResourcePool.inventory.HasType(ResourceType.Food, 10) || city.population >= city.maxPopulation) && !GameController.debug)
                button.interactable = false;
        }
        else
        {
            if ((city != null && ResourcePool.inventory.HasType(ResourceType.Food, 10) && city.population < city.maxPopulation) || GameController.debug)
                button.interactable = true;
        }
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIHandler.DisplayTooltip("Add 1 Population\nCost: 10 Food");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIHandler.HideTooltip();
    }
}
