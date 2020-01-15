using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour {

    Transform target;

    private void Awake()
    {
        target = Camera.main.transform;
    }

    void Update () {
        transform.rotation = Quaternion.Euler(0f, 0f, target.rotation.eulerAngles.y);	
	}
}
