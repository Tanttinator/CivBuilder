using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TechnologyTabUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    string tooltip;


    public void Setup(Technology tech)
    {
        tooltip = "Cost: " + tech.cost + tech.description;
        GetComponent<Button>().onClick.AddListener(delegate { Technology.SetCurrentTechnology(tech); });
        GetComponentInChildren<Text>().text = tech.name;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIHandler.DisplayTooltip(tooltip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIHandler.HideTooltip();
    }
}
