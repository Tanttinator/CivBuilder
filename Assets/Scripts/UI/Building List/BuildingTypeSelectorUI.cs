using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingTypeSelectorUI : MonoBehaviour {

    public Image notification;
    Transform list;
    [HideInInspector] public bool isOpen = true;

    List<BuildingTabUI> notifications = new List<BuildingTabUI>();

    public GameObject listPrefab;

    static BuildingTypeSelectorUI activeList;

    private void OnDisable()
    {
        Destroy(list.gameObject);
        Destroy(gameObject);
    }

    public void Setup(BuildingType type, Transform content)
    {
        list = Instantiate(listPrefab, content).transform;
        GetComponentInChildren<Text>().text = type.ToString();
        notification.enabled = false;
        isOpen = true;
    }

    public void AddBuildingTab(GameObject tab)
    {
        tab.transform.SetParent(list.GetComponent<TypeListUI>().content, false);
    }

    public void Show()
    {
        if (isOpen)
            return;
        isOpen = true;
        list.localScale = new Vector3(1, 1, 1);
        if(activeList != null)
            activeList.Hide();
        activeList = this;
    }

    public void Hide()
    {
        if (!isOpen)
            return;
        isOpen = false;
        list.localScale = new Vector3(0, 0, 0);
        activeList = null;
    }

    public void AddNotification(BuildingTabUI tab)
    {
        if (notifications.Contains(tab))
            return;
        if (notifications.Count == 0)
        {
            ShowNotification();
        }
        notifications.Add(tab);
    }

    public void RemoveNotification(BuildingTabUI tab)
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
        notification.enabled = true;
        FindObjectOfType<BuildingListUI>().AddNotification(this);
    }

    public void HideNotification()
    {
        if (!notification.enabled)
            return;
        notification.enabled = false;
        FindObjectOfType<BuildingListUI>().RemoveNotification(this);
    }
}
