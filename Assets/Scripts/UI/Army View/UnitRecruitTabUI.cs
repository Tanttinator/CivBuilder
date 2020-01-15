using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitRecruitTabUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    Unit unit;
    Button button;
    string tooltip;

    public void Setup(Unit unit, Army army)
    {
        this.unit = unit;
        button = GetComponent<Button>();
        button.onClick.AddListener(delegate { army.AddUnit(Unit.CreateUnit(unit)); });
        GetComponentInChildren<Text>().text = unit.name;
        tooltip = "Cost:\n" + unit.size + " manpower.";
        foreach (ResourceStack stack in unit.cost)
            tooltip += "\n" + stack;
    }

	void Update () {
        if (gameObject.activeSelf)
        {
            if (button.interactable)
            {
                if (Worker.unemployed < unit.size || !ResourcePool.inventory.HasResources(unit.cost))
                    button.interactable = false;
            }
            else
            {
                if (Worker.unemployed >= unit.size && ResourcePool.inventory.HasResources(unit.cost))
                    button.interactable = true;
            }
        }
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
