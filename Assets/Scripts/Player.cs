using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : Target
{
    public Transform healthBar;
    public SpriteRenderer[] srs;
    public SpriteRenderer[] blockOverlay;

    void Start() {
        srs = this.GetComponentsInChildren<SpriteRenderer>();
        blockOverlay = new SpriteRenderer[2]{
            srs[2], srs[3]
        };
        
        health = 100;
        block = 0;
    }

    void Update() {

    }


}
