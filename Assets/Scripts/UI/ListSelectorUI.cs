using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ListSelectorUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    UIHandler UI;

    public Toggle toggle;
    public GameObject list;
    public Image notification;
    public string listName;

    int notifications = 0;

    Action onNotificationDisplayed;

	void Start () {
        notification.gameObject.SetActive(false);
        UI = FindObjectOfType<UIHandler>();
        toggle.onValueChanged.AddListener(OnValueChanged);
        Hide();
	}

    void OnValueChanged(bool value)
    {
        if (value)
        {
            if (UI.currentList != null)
            {
                UI.currentList.Hide();
                UI.currentList = this;
            }
            Show();
        }
        else
        {
            UI.currentList = null;
            Hide();
        }
    }

    public void Hide()
    {
        list.transform.localScale = new Vector3(0, 0, 0);
    }

    public void Show()
    {
        list.transform.localScale = new Vector3(1, 1, 1);
    }

    public void DisplayNotification()
    {
        notifications++;
        if (onNotificationDisplayed != null && !toggle.isOn)
            onNotificationDisplayed();
        notification.gameObject.SetActive(true);
    }

    public void HideNotification()
    {
        if(notifications > 0)
            notifications--;
        if (notifications == 0)
        {
            notification.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIHandler.DisplayTooltip(listName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIHandler.HideTooltip();
    }

    public void RegisterOnNotificationDisplayed(Action callback)
    {
        onNotificationDisplayed += callback;
    }
}
