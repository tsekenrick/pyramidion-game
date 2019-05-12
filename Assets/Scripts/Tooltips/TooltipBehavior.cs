using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipBehavior : MonoBehaviour {
    
    void Update() {
        Vector3 mousePos = Input.mousePosition;
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
    }
}
