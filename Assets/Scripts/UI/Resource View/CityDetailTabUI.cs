using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityDetailTabUI : MonoBehaviour {

    public Text nameText;
    public Text amountText;
    public Text incomeText;

    public void Setup(City city)
    {
        nameText.text = city.name;
        amountText.text = "" + 0;
        incomeText.text = "" + 0;
    }
}
