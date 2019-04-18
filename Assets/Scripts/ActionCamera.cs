using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCamera : MonoBehaviour {
    void Update() {
        transform.LookAt(GameObject.Find("_CameraTarget").transform);
    }
}
