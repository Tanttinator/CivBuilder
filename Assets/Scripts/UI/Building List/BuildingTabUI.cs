using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class BuildingTabUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Text nameText;
    public Image notification;
    public RawImage icon;
    public Text cost;
    Button button;

    Building building;
    string tooltip;

    BuildingTypeSelectorUI selector;

	public void Setup(Building building, BuildingTypeSelectorUI selector)
    {
        this.building = building;
        this.selector = selector;
        nameText.text = building.name;
        button = GetComponent<Button>();
        button.onClick.AddListener(delegate { UIHandler.HideTooltip(); BuildingHandler.instance.StartPlaceBuilding(building); });
        tooltip = building.description;
        notification.enabled = false;
        UpdateCostText();
        ShowNotification();
    }

    void UpdateCostText()
    {
        string costText = "";
        if (building.cost.Length > 0)
        {
            costText += "Cost:";
            foreach (ResourceStack stack in building.cost)
            {
                costText += "\n<color=" + (ResourcePool.inventory.HasResources(stack)? "black" : "red") + ">" + stack.resource + " " + stack.amount + "</color>";
            }
        }
        icon.texture = AssetPreview.GetAssetPreview(building.prefab);
        cost.text = costText;
    }

    private void Update()
    {
        UpdateCostText();
        if (button.interactable)
        {
            if ((!ResourcePool.inventory.HasResources(building.cost) || (building.unique && Building.GetBuildingsOfType(building).Length > 0)) && !GameController.debug)
                button.interactable = false;
        }
        else
        {
            if ((ResourcePool.inventory.HasResources(building.cost) && !(building.unique && Building.GetBuildingsOfType(building).Length > 0)) || GameController.debug)
                button.interactable = true;
        }
    }

    public void ShowNotification()
    {
        if (notification.enabled)
            return;
        notification.enabled = true;
        selector.AddNotification(this);
    }

    void HideNotification()
    {
        if (!notification.enabled)
            return;
        notification.enabled = false;
        selector.RemoveNotification(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIHandler.DisplayTooltip(tooltip);
        HideNotification();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIHandler.HideTooltip();
    }
}
