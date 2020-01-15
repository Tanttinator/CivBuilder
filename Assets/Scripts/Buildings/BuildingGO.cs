using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGO : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = collision.gameObject;
        if (go.tag == "TerrainObject")
            Destroy(go);
    }
}
