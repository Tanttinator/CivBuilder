using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ArmyViewUI : MonoBehaviour {

    public GameObject armyTabGO;
    public Transform armyTabList;
    public Dictionary<Army, GameObject> armyTabs = new Dictionary<Army, GameObject>();

    WindowUI window;

    [Header("Army Info")]
    public GameObject armyInfoPanel;
    public Transform armyUnitList;
    public Text armyName;
    public Text armyLeader;
    public Text armyStrength;
    public Text armyUnitCount;
    Army activeArmy;
    public Dictionary<Unit, GameObject> armyUnitTabs = new Dictionary<Unit, GameObject>();
    public GameObject unitTab;

    [Header("Recruit")]
    public GameObject recruitTab;
    public GameObject recruitView;
    public Transform recruitList;
    public Dictionary<Unit, GameObject> recruitUnitTabs = new Dictionary<Unit, GameObject>();
    public Button openRecruit;

    public void RegisterCallbacks()
    {
        window = GetComponent<WindowUI>();
        window.RegisterOnHide(Hide);
        window.RegisterOnShow(Show);
        Army.RegisterOnArmyCreated(CreateArmyTab);
        Army.RegisterOnArmyDeleted(DeleteArmyTab);
    }

    private void OnDisable()
    {
        Army.UnregisterOnArmyCreated(CreateArmyTab);
        Army.UnregisterOnArmyDeleted(DeleteArmyTab);

        foreach (Army army in armyTabs.Keys)
            Destroy(armyTabs[army]);
        armyTabs.Clear();

        foreach (Unit unit in armyUnitTabs.Keys)
            Destroy(armyUnitTabs[unit]);
        armyUnitTabs.Clear();

        foreach (Unit unit in recruitUnitTabs.Keys)
            Destroy(recruitUnitTabs[unit]);
        recruitUnitTabs.Clear();
    }

    public void Show()
    {
        if (Army.armies.Count > 0)
            armyTabs[Army.armies[0]].GetComponent<Toggle>().isOn = true;
        else
            armyInfoPanel.SetActive(false);

        CloseRecruit();
    }

    public void Hide()
    {
        GetComponentInChildren<ToggleGroup>().SetAllTogglesOff();
    }

    void OpenArmyInfo(Army army, bool isOpen)
    {
        if (isOpen)
        {
            if (activeArmy != null)
                CloseArmyInfo(activeArmy);
            activeArmy = army;
            foreach (Unit unit in army.units)
                AddUnitTabToArmy(unit);
            armyName.text = army.name;
            armyLeader.text = "Leader: " + army.leader.name;
            armyStrength.text = "Strength: " + army.Strength;
            armyUnitCount.text = army.units.Count + "/" + army.maxUnits;
            activeArmy.RegisterOnUnitAdded(AddUnitTabToArmy);
            activeArmy.RegisterOnUnitAdded(UpdateCurrentArmyInfo);
            activeArmy.RegisterOnUnitRemoved(RemoveUnitTabFromArmy);
            activeArmy.RegisterOnUnitRemoved(UpdateCurrentArmyInfo);
            armyInfoPanel.SetActive(true);
        }
        else
        {
            CloseArmyInfo(army);
        }
    }

    void CloseArmyInfo(Army army)
    {
        if (activeArmy == army)
        {
            while(armyUnitTabs.Count > 0)
                RemoveUnitTabFromArmy(armyUnitTabs.First().Key);
            activeArmy.UnregisterOnUnitAdded(AddUnitTabToArmy);
            activeArmy.UnregisterOnUnitAdded(UpdateCurrentArmyInfo);
            activeArmy.UnregisterOnUnitRemoved(RemoveUnitTabFromArmy);
            activeArmy.UnregisterOnUnitRemoved(UpdateCurrentArmyInfo);
            activeArmy = null;
            armyInfoPanel.SetActive(false);
        }
    }

    void UpdateCurrentArmyInfo(Unit unit)
    {
        armyName.text = activeArmy.name;
        armyLeader.text = "Leader: " + activeArmy.leader.name;
        armyStrength.text = "Strength: " + activeArmy.Strength;
        armyUnitCount.text = activeArmy.units.Count + "/" + activeArmy.maxUnits;
    }

    void CreateArmyTab(Army army)
    {
        GameObject tab = Instantiate(armyTabGO, armyTabList);
        tab.GetComponentInChildren<Text>().text = army.name;
        Toggle toggle = tab.GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate { OpenArmyInfo(army, toggle.isOn); });
        toggle.isOn = false;
        armyTabs.Add(army, tab);
    }

    void DeleteArmyTab(Army army)
    {
        Destroy(armyTabs[army]);
        armyTabs.Remove(army);
    }

    void AddUnitTabToArmy(Unit unit)
    {
        GameObject tab = Instantiate(unitTab, armyUnitList);
        tab.GetComponentInChildren<Text>().text = unit.name;
        armyUnitTabs.Add(unit, tab);
    }

    void RemoveUnitTabFromArmy(Unit unit)
    {
        Destroy(armyUnitTabs[unit]);
        armyUnitTabs.Remove(unit);
    }

    void AddUnitTabToRecruit(Unit unit)
    {
        GameObject tab = Instantiate(recruitTab, recruitList);
        tab.GetComponent<UnitRecruitTabUI>().Setup(unit, activeArmy);
        recruitUnitTabs.Add(unit, tab);
    }

    public void OpenRecruit()
    {
        if (activeArmy != null)
        {
            foreach (Unit unit in activeArmy.center.trainableUnits)
                AddUnitTabToRecruit(unit);
        }
        recruitView.GetComponent<WindowUI>().Show();
    }

    public void CloseRecruit()
    {
        recruitView.GetComponent<WindowUI>().Hide();
        foreach (Unit unit in recruitUnitTabs.Keys)
            Destroy(recruitUnitTabs[unit]);
        recruitUnitTabs.Clear();
    }

}
