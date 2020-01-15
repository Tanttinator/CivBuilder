using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class BuildingListUI : MonoBehaviour {

    public Transform content;
    public Transform menu;

    public Image notification;

    public GameObject typeSelector;
    Dictionary<BuildingType, GameObject> selectors = new Dictionary<BuildingType, GameObject>();

    List<BuildingTypeSelectorUI> notifications = new List<BuildingTypeSelectorUI>();

    public GameObject buildingTab;

    static Action onNotificationDisplayed;

    public void PreStart()
    {
        foreach(BuildingType type in System.Enum.GetValues(typeof(BuildingType)))
        {
            AddType(type);
            selectors[type].GetComponent<BuildingTypeSelectorUI>().Hide();
        }
        selectors.First().Value.GetComponent<BuildingTypeSelectorUI>().Show();
        notification.enabled = false;
    }

    public void RegisterCallbacks()
    {
        Building.RegisterOnBuildingDiscovered(AddBuilding);
    }

    private void OnDisable()
    {
        Building.UnregisterOnBuildingDiscovered(AddBuilding);
        foreach (GameObject go in selectors.Values)
            Destroy(go);
        selectors.Clear();
    }

    void AddType(BuildingType type)
    {
        GameObject buttonGO = Instantiate(typeSelector, menu);
        buttonGO.GetComponent<BuildingTypeSelectorUI>().Setup(type, content);
        selectors.Add(type, buttonGO);
    }

    void AddBuilding(Building building)
    {
        GameObject tabGO = Instantiate(buildingTab);
        tabGO.GetComponent<BuildingTabUI>().Setup(building, selectors[building.type].GetComponent<BuildingTypeSelectorUI>());
        selectors[building.type].GetComponent<BuildingTypeSelectorUI>().AddBuildingTab(tabGO);
    }

    public void AddNotification(BuildingTypeSelectorUI tab)
    {
        if (notifications.Contains(tab))
            return;
        if (notifications.Count == 0)
        {
            ShowNotification();
        }
        notifications.Add(tab);
    }

    public void RemoveNotification(BuildingTypeSelectorUI tab)
    {
        if (!notifications.Contains(tab))
            return;
        notifications.Remove(tab);
        if (notifications.Count == 0)
            HideNotification();
    }

    public void ShowNotification()
    {
        if (notification.enabled)
            return;
        if (onNotificationDisplayed != null)
            onNotificationDisplayed();
        notification.enabled = true;
    }

    void HideNotification()
    {
        if (!notification.enabled)
            return;
        notification.enabled = false;
    }

    public static void RegisterOnNotificationDisplayed(Action callback)
    {
        onNotificationDisplayed += callback;
    }

    public static void UnregisterOnNotificationDisplayed(Action callback)
    {
        onNotificationDisplayed -= callback;
    }
}
