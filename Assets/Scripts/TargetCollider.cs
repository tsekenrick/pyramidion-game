using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TargetCollider : MonoBehaviour
{
    public Camera actionCam; 
    public Camera uiCam;
    public SpriteRenderer ren; //set in inspector
    private BoxCollider2D box;

    void Start() { 
        uiCam = Camera.main;
        actionCam = GameObject.Find("Perspective Camera").GetComponent<Camera>();
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
        transform.position = .5f * (min + max);
        box.size = 1.5f * (max - min);
    }

    void OnMouseEnter() {
        if(Board.instance.curPhase == Phase.Resolution || Board.instance.curPhase == Phase.Event) return;
        TextMeshPro tmp = transform.parent.Find("EnemyName").GetComponent<TextMeshPro>();
        tmp.DOColor(new Color(tmp.color.r, tmp.color.g, tmp.color.b, 1f), .5f);
    }

    void OnMouseExit() {
        if(Board.instance.curPhase == Phase.Resolution || Board.instance.curPhase == Phase.Event) return;
        TextMeshPro tmp = transform.parent.Find("EnemyName").GetComponent<TextMeshPro>();
        tmp.DOColor(new Color(tmp.color.r, tmp.color.g, tmp.color.b, 0), .5f);
    }

}
