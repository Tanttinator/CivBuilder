using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGO_Grindstone : MonoBehaviour {

    bool isWorking = false;
    float speed = 15f;
    public Transform target;

    private void Update()
    {
        if (isWorking)
            target.Rotate(new Vector3(0, 0, Time.deltaTime * speed / GameController.gameTick));
    }

    public void OnWorkStart()
    {
        isWorking = true;
    }

    public void OnWorkEnd()
    {
        isWorking = false;
    }
}
