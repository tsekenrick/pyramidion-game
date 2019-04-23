using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCollider : MonoBehaviour
{
    public Camera actionCam; //set in inspector
    public Camera uiCam; //set in inspector
    public SpriteRenderer ren; //set in inspector
    private BoxCollider2D box;

    void Start() { 
        box = GetComponent<BoxCollider2D>(); 
    }

    void LateUpdate() {
        //convert sprite bounds to pixel coords on actionCam
        Vector3 min = actionCam.WorldToScreenPoint(ren.bounds.min);
        Vector3 max = actionCam.WorldToScreenPoint(ren.bounds.max);

        //convert to world coords relative to uiCam
        min = uiCam.ScreenToWorldPoint(min);
        max = uiCam.ScreenToWorldPoint(max);

        //move and smoosh the trigger collider
        transform.position = 0.5f*(min + max);
        box.size =0.5f*(max-min);
    }

    void OnMouseUp(){
        
    }
}
