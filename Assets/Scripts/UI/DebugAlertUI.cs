using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAlertUI : MonoBehaviour {

	void Update () {
		if(transform.localScale == new Vector3(0, 0, 0))
        {
            if (GameController.debug)
                transform.localScale = new Vector3(1, 1, 1);
        }else if(transform.localScale == new Vector3(1, 1, 1))
        {
            if (!GameController.debug)
                transform.localScale = new Vector3(0, 0, 0);
        }
	}

}
