using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfoUI : MonoBehaviour {

    public Text nameText;
    public Text descriptionText;

    [Header("Work List")]
    public BuildingInfoListUI workListGO;
    public Button workListSelector;

    [Header("Needs List")]
    public BuildingInfoListUI needsListGO;
    public Button needsListSelector;

    public void Show(Building building)
    {
        nameText.text = building.name;
        descriptionText.text = building.description;

        if (building is Residential)
        {
            ShowNeedsList(building);
        }
        else
        {
            HideNeedsList();
        }
        if (building.maxWorkers > 0)
        {
            ShowWorkList(building);
        }
        else
        {
            HideWorkList();
        }

        GetComponent<WindowUI>().Show();
    }

    void ShowNeedsList(Building building)
    {
        needsListGO.Show();
        needsListSelector.gameObject.SetActive(true);
        needsListGO.GetComponent<NeedsListUI>().Setup(building as Residential);
    }

    void HideNeedsList()
    {
        needsListGO.Hide();
        needsListSelector.gameObject.SetActive(false);
    }

    void ShowWorkList(Building building)
    {
        workListGO.Show();
        workListSelector.gameObject.SetActive(true);
        workListGO.GetComponent<WorkListUI>().Setup(building);
    }

    void HideWorkList()
    {
        workListGO.Hide();
        workListSelector.gameObject.SetActive(false);
    }
}
