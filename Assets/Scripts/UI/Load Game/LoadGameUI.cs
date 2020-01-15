using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LoadGameUI : MonoBehaviour {

    public GameObject saveTab;
    public Transform saveList;

    Dictionary<GameData, GameObject> tabs = new Dictionary<GameData, GameObject>();

	public void Show()
    {
        while(tabs.Count > 0)
        {
            RemoveTab(tabs.First().Key);
        }

        foreach(GameData save in GameController.saves.Values)
        {
            GameObject tab = Instantiate(saveTab, saveList);
            tab.GetComponent<SaveTabUI>().Setup(this, save);
            tabs.Add(save, tab);
        }
    }

    public void RemoveTab(GameData data)
    {
        Destroy(tabs[data]);
        tabs.Remove(data);
    }
}
