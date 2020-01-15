using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {

    public Canvas canvas;

    [HideInInspector] public ListSelectorUI currentList;

    static GameObject tooltip;
    static RectTransform tooltipTransform;

    public GameObject resourcePopup;

    public GameObject mainMenu;
    public bool isMenuOpen = false;

    public static UIHandler instance;

    public void Awake()
    {
        tooltip = GameObject.Find("Tooltip");
        tooltipTransform = tooltip.GetComponent<RectTransform>();

        instance = this;
    }

    public void OnStart()
    {
        tooltip.SetActive(false);

        mainMenu.SetActive(false);
    }

    public void RegisterCallbacks()
    {
        ResourcePool.RegisterOnResourceAddedByBuilding(CreateResourcePopup);
    }

    void OnDisable()
    {
        ResourcePool.UnregisterOnResourceAddedByBuilding(CreateResourcePopup);
    }

    private void Update()
    {
        if (tooltip.activeInHierarchy)
        {
            float offset = 10f / canvas.scaleFactor;
            float width = tooltipTransform.rect.width * canvas.scaleFactor;
            float height = tooltipTransform.rect.height * canvas.scaleFactor;
            float x = ((width / 2f) + offset);
            float y = height / 2f;
            tooltip.transform.position = new Vector3(   
                                    Input.mousePosition.x > width + offset? -x : x,
                                    Input.mousePosition.y > height? -y : y,
                                    0
                                    ) + Input.mousePosition;
        }
    }

    public void OpenMenu()
    {
        mainMenu.SetActive(true);
        isMenuOpen = true;
        GameController.paused = true;
    }

    public void CloseMenu()
    {
        mainMenu.SetActive(false);
        isMenuOpen = false;
        GameController.paused = false;
    }

    public void SaveAndClose()
    {
        GameController.CloseGame();
    }

    public static void DisplayTooltip(string text)
    {
        tooltip.SetActive(true);
        tooltip.GetComponentInChildren<Text>().text = text;
        tooltip.transform.SetAsLastSibling();
    }

    public static void HideTooltip()
    {
        tooltip.SetActive(false);
    }

    public void CreateResourcePopup(ResourceStack[] stack, Vector3 location)
    {
        GameObject popup = Instantiate(resourcePopup, canvas.transform);
        popup.GetComponent<ResourcePopupUI>().Setup(stack, location);
        popup.transform.SetAsFirstSibling();
    }


}
