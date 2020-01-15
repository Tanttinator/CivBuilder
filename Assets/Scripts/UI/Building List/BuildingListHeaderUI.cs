using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingListHeaderUI : MonoBehaviour, IPointerClickHandler {

    bool isOpen = false;

    public RectTransform parent;
    public Transform icon;
    float size = 0f;

    Vector2 down = new Vector2(-212, -260);
    Vector2 up = new Vector2(-212, 0);

    private void Awake()
    {
        parent.anchoredPosition = down;
    }

    private void Update()
    {
        if (isOpen)
        {
            if (size < 0.5f)
                size = Mathf.Min(1f, size + Time.deltaTime);
        }else if(size > 0f)
        {
            size = Mathf.Max(0f, size - Time.deltaTime);
        }
        parent.anchoredPosition = new Vector2(up.x, Mathf.Lerp(down.y, up.y, size / 0.5f));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isOpen = !isOpen;
        icon.rotation = isOpen ? Quaternion.Euler(0f, 0f, 0f) : Quaternion.Euler(0f, 0f, 180f);
    }
}
