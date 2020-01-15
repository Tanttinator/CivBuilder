using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechProgressUI : MonoBehaviour {

    public Text nameText;
    public Text progressText;
    Technology currentTech;

    public void RegisterCallbacks()
    {
        Technology.RegisterOnCurrentTechSet(SetTechnology);
    }

    private void OnDisable()
    {
        Technology.UnregisterOnCurrentTechSet(SetTechnology);
        if (currentTech != null)
            currentTech.UnregisterOnProgress(UpdateProgress);
    }

    public void SetTechnology(Technology tech)
    {
        if(currentTech != null)
        {
            currentTech.UnregisterOnProgress(UpdateProgress);
        }
        tech.RegisterOnProgress(UpdateProgress);
        currentTech = tech;
        nameText.text = tech.name;
        UpdateProgress();
    }

    public void UpdateProgress()
    {
        int progress = 0;
        if (currentTech.cost == 0)
            progress = 100;
        else
            progress = Mathf.FloorToInt(currentTech.progress / (currentTech.cost * 1f) * 100);
        progressText.text = "Progress: " + (progress == 100 ? "Completed" : progress + "%");
    }
}
