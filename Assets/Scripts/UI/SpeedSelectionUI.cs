using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedSelectionUI : MonoBehaviour {

    public Toggle pause;
    public Toggle x1;
    public Toggle x2;
    public Toggle x3;
    public Toggle debug;

    private void Awake()
    {
        pause.onValueChanged.AddListener(delegate { Pause(pause.isOn); ChangeColor(pause); });
        pause.isOn = false;

        x1.onValueChanged.AddListener(delegate { SetSpeed(2, x1.isOn); ChangeColor(x1); });
        x1.isOn = true;
        x1.onValueChanged.Invoke(true);

        x2.onValueChanged.AddListener(delegate { SetSpeed(4, x2.isOn); ChangeColor(x2); });
        x2.isOn = false;

        x3.onValueChanged.AddListener(delegate { SetSpeed(6, x3.isOn); ChangeColor(x3); });
        x3.isOn = false;

        debug.onValueChanged.AddListener(delegate { SetSpeed(10, debug.isOn); ChangeColor(debug); });
        debug.isOn = false;
    }

    void ChangeColor(Toggle toggle)
    {
        toggle.GetComponent<Image>().color = toggle.isOn ? new Color(0.5f, 0.5f, 0.5f) : new Color(1, 1, 1);
    }

    void SetSpeed(int speed, bool b)
    {
        if (b)
            GameController.SetGameSpeed(speed);
    }

    void Pause(bool paused)
    {
        GameController.paused = paused;
    }
}
