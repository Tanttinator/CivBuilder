using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfoListUI : MonoBehaviour {

    public Transform content;
    bool isOpen = true;

    static BuildingInfoListUI activeList;

    public void Show()
    {
        if (isOpen)
            return;
        isOpen = true;
        if (activeList != null && activeList != this)
            activeList.Hide();
        activeList = this;
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void Hide()
    {
        if (!isOpen)
            return;
        isOpen = false;
        activeList = null;
        transform.localScale = new Vector3(0, 0, 0);
    }

    public void Toggle()
    {
        if (isOpen)
            Hide();
        else
            Show();
    }

}
