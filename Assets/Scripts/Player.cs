using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : Target
{
    
    private const float MAX_HEALTH = 100.0f;

    void Start() {
        srs = this.GetComponentsInChildren<SpriteRenderer>();
        blockOverlay = new SpriteRenderer[2]{
            srs[3], srs[4]
        };

        healthBar = srs[2].GetComponent<Transform>();
        foreach(SpriteRenderer sr in blockOverlay) sr.enabled = false;
        
        health = 100;
        block = 0;
    }

    void Update() {
        foreach(SpriteRenderer sr in blockOverlay) sr.enabled = (block > 0);
        healthBar.DOScaleX(Mathf.Max(0, (float)health/MAX_HEALTH), .3f);

        // if(Board.me.curPhase == Phase.Mulligan) {
        //     this.GetComponent<SpriteRenderer>().sprite = combatStates[0];
        // }
    }


}
