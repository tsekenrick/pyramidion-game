using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : Target
{
    public Transform healthBar;
    public SpriteRenderer[] srs;
    public SpriteRenderer[] blockOverlay;
    private const int MAX_HEALTH = 100;

    void Start() {
        srs = this.GetComponentsInChildren<SpriteRenderer>();
        blockOverlay = new SpriteRenderer[2]{
            srs[2], srs[3]
        };

        foreach(SpriteRenderer sr in blockOverlay) sr.enabled = false;
        health = 100;
        block = 0;
    }

    void Update() {
        foreach(SpriteRenderer sr in blockOverlay) sr.enabled = (block > 0);
        healthBar.DOScaleX(Mathf.Max(0, health/MAX_HEALTH), .3f);    
    }


}
