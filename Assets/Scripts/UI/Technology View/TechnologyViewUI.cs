using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechnologyViewUI : MonoBehaviour {

    public GameObject techTab;
    public Transform techList;
    public Dictionary<Technology, GameObject> techTabs = new Dictionary<Technology, GameObject>();
    Technology currentTech;

    public void RegisterCallbacks()
    {
        Technology.RegisterOnTechUnlocked(AddTechnology);
        Technology.RegisterOnTechDiscovered(RemoveTechnology);
        Technology.RegisterOnCurrentTechSet(SetCurrentTechnology);
    }

    public void OnPostLoad()
    {
        Technology.RegisterOnTechDiscovered(Show);
    }

    private void OnDisable()
    {
        Technology.UnregisterOnTechUnlocked(AddTechnology);
        Technology.UnregisterOnTechDiscovered(RemoveTechnology);
        Technology.UnregisterOnTechDiscovered(Show);
        Technology.UnregisterOnCurrentTechSet(SetCurrentTechnology);

        foreach (GameObject go in techTabs.Values)
            Destroy(go);
        techTabs.Clear();
    }

    void AddTechnology(Technology tech)
    {
        GameObject tabGO = Instantiate(techTab, techList);
        tabGO.GetComponent<TechnologyTabUI>().Setup(tech);
        techTabs.Add(tech, tabGO);
    }

    void RemoveTechnology(Technology tech)
    {
        Destroy(techTabs[tech]);
        techTabs.Remove(tech);
    }

    void SetCurrentTechnology(Technology tech)
    {
        if(currentTech != null && techTabs.ContainsKey(currentTech))
            techTabs[currentTech].GetComponent<Button>().interactable = true;
        techTabs[tech].GetComponent<Button>().interactable = false;
        currentTech = tech;
    }

    void Show(Technology tech)
    {
        GetComponent<WindowUI>().Show();
    }
}
