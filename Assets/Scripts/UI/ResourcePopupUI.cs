using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePopupUI : MonoBehaviour {

    public Text text;
    public float fadeTime;
    float time;
    public float riseDist;
    Vector3 origin;
    RectTransform rt;
    Canvas canvas;

    public void Setup(ResourceStack[] resource, Vector3 location)
    {
        text.text = "";
        foreach(ResourceStack stack in resource)
            text.text += stack + "\n";
        origin = location;
        rt = GetComponent<RectTransform>();
        canvas = UIHandler.instance.canvas;
    }

	void Update () {
        time += Time.deltaTime / GameController.gameTick;
        float x = Camera.main.WorldToScreenPoint(origin).x / canvas.scaleFactor;
        float y = (Camera.main.WorldToScreenPoint(origin).y + Mathf.Lerp(0f, riseDist, time / fadeTime) + 10f) / canvas.scaleFactor;
        rt.anchoredPosition = new Vector2(x, y);
        text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(1f, 0f, time / fadeTime));
        if (time >= fadeTime)
            Destroy(gameObject);
	}
}
