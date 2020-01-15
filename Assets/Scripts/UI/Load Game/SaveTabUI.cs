using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SaveTabUI : MonoBehaviour{

    public Button delete;
    public Text nameText;
    public Text lastPlayedText;

    public void Setup(LoadGameUI load, GameData data)
    {
        nameText.text = data.name;
        lastPlayedText.text = "Played: " + (data.lastPlayed.Date == DateTime.Now.Date ? "Today " + data.lastPlayed.ToString("HH:mm") : data.lastPlayed.ToString("dd MM yyyy HH:mm"));
        delete.onClick.AddListener(delegate { load.RemoveTab(data); GameController.DeleteSave(data); });
        GetComponent<Button>().onClick.AddListener(delegate { GameController.OpenGame(data); });
    }

}
