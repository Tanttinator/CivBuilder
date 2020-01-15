using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WindowUI : MonoBehaviour {
    
    public bool hideOnAwake = false;
    public bool isInActiveWindowPool = true;

    [HideInInspector] public bool isOpen = true;
    static WindowUI activeWindow = null;

    Action onShow;
    Action onHide;

    private void Awake()
    {
        if (hideOnAwake)
            Hide();
    }

    public void Show()
    {
        if (isOpen)
            return;
        isOpen = true;
        transform.localScale = new Vector3(1, 1, 1);
        InputHandler.escActionStack.Add(Hide);
        if (isInActiveWindowPool)
        {
            if (activeWindow != null)
                activeWindow.Hide();
            activeWindow = this;
        }
        if (onShow != null)
            onShow();
    }

    public void Hide()
    {
        if (!isOpen)
            return;
        isOpen = false;
        if(isInActiveWindowPool)
            activeWindow = null;
        transform.localScale = new Vector3(0, 0, 0);
        InputHandler.escActionStack.Remove(Hide);
        if (onHide != null)
            onHide();
    }

    public void Toggle()
    {
        if (isOpen)
            Hide();
        else
            Show();
    }

    public void RegisterOnShow(Action callback)
    {
        onShow += callback;
    }

    public void RegisterOnHide(Action callback)
    {
        onHide += callback;
    }
}
