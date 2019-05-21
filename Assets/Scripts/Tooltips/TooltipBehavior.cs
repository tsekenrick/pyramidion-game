using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipBehavior : MonoBehaviour {
    public float yOffset = 0f;

    void Update() {
        Vector3 mousePos = Input.mousePosition;
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
        transform.position = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);

    }
}
