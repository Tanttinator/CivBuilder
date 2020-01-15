using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MorePopulationButtonUI : MonoBehaviour {

    bool isOpen = false;

    public RectTransform icon;
    public Transform content;

    private void Awake()
    {
        isOpen = false;
        Close();
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        if (isOpen)
            Open();
        else
            Close();
    }

    void Open()
    {
        icon.rotation = Quaternion.Euler(0f, 0f, 180f);
        content.localScale = new Vector3(1, 1, 1);
        content.GetComponent<LayoutElement>().preferredHeight = 60f;
    }

    void Close()
    {
        icon.rotation = Quaternion.Euler(0f, 0f, 0f);
        content.localScale = new Vector3(0, 0, 0);
        content.GetComponent<LayoutElement>().preferredHeight = 0f;
    }
}
