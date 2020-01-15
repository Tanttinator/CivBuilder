using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsumableTabUI : MonoBehaviour {

    public Text nameText;
    public Transform progressBar;
    public Text valueText;
    Consumable consumable;

	public void Setup(Consumable consumable)
    {
        this.consumable = consumable;
        nameText.text = "" + (consumable.resource == null ? consumable.type.ToString() : consumable.resource.name);
    }

    private void Update()
    {
        if(consumable != null)
        {
            valueText.text = consumable.satisfaction + "%";
            progressBar.localScale = new Vector3(consumable.satisfaction / 100f, 1f, 1f);
        }
    }
}
