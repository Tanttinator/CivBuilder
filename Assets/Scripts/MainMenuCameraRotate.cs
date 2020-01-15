using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraRotate : MonoBehaviour {

    public float speed;

	void Update () {
        transform.Rotate(new Vector3(0, Time.deltaTime * speed, 0));	
	}
}
