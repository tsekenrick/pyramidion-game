using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Player : Target
{
    public TextMeshPro[] healthText;
    public const int MAX_HEALTH = 100;

    void Start() {
        startPos = this.transform.position;
        healthText = GetComponentsInChildren<TextMeshPro>();
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
        health = Mathf.Min(health, MAX_HEALTH);
        foreach(SpriteRenderer sr in blockOverlay) sr.enabled = (block > 0);
        healthBar.DOScaleX(Mathf.Max(0, (float)health/MAX_HEALTH), 1.25f);

        healthText[0].text = health > 0 ? $"{health}/{MAX_HEALTH}" : $"0/{MAX_HEALTH}";
        healthText[1].text = block > 0 ? block.ToString() : " ";
        // if(Board.me.curPhase == Phase.Mulligan) {
        //     this.GetComponent<SpriteRenderer>().sprite = combatStates[0];
        // }
    }


}
