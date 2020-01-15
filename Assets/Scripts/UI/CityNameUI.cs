using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CityNameUI : MonoBehaviour, IPointerClickHandler {

    public Text nameText;
    [HideInInspector] public Vector3 position;

    City city;

    RectTransform rt;
    Canvas canvas;

    public void Setup(City city, Vector3 position)
    {
        this.city = city;
        transform.SetAsFirstSibling();
        nameText.text = city.name;
        this.position = position;
        rt = GetComponent<RectTransform>();
        city.RegisterOnCenterChanged(SetPosition);
        canvas = FindObjectOfType<Canvas>();
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }

    void SetPosition(Vector3 position)
    {
        this.position = position;
    }

	void Update () {
        rt.anchoredPosition = Camera.main.WorldToScreenPoint(position) / canvas.scaleFactor  + new Vector3(0, 50f, 0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<CityInfoPanelUI>().Show(city);
    }
}
