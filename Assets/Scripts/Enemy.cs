﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : Target
{
    private Board board;

    public List<EnemyAction> prevActions;
    public List<EnemyAction> curActions;

    // view related
    public Transform healthBar;
    public SpriteRenderer[] srs;
    public SpriteRenderer[] blockOverlay;
    public Sprite[] mulliganIntentIcons;
    private const float MAX_HEALTH = 60f;
    void Start() {
        board = Board.me;

        health = 60;
        block = 0;

        prevActions = new List<EnemyAction>();
        curActions = new List<EnemyAction>();

        srs = this.GetComponentsInChildren<SpriteRenderer>();
        blockOverlay = new SpriteRenderer[2]{
            srs[3], srs[4]
        };

        healthBar = srs[2].GetComponent<Transform>();

        foreach(SpriteRenderer sr in blockOverlay) sr.enabled = false;
    }

    void Update() {
        // healthbar/block overlay logic
        foreach(SpriteRenderer sr in blockOverlay) sr.enabled = (block > 0);
        healthBar.DOScaleX(Mathf.Max(0, (float)health/MAX_HEALTH), .3f);

        switch(board.curPhase) {
            case Phase.Mulligan:               
                // roll for action type and display non-numerical intent
                if(curActions.Count == 0) {
                    for(int i = 0; i < Random.Range(2, 3); i++) { 
                        ActionType actionType = (ActionType)Random.Range(0, 2); // change to max 3 when we added statuses
                        // temporary cheeky one-liner - will not work for ActionType.Status
                        GameObject target = actionType == ActionType.Attack ? GameObject.Find("Player") : this.gameObject;
                        curActions.Add(new EnemyAction(actionType, target, this.gameObject));
                        Debug.Log($"enemy will take action of type: {curActions[i].actionType}");
                    }
                }
                break;

            case Phase.Play:
                break;
            
            case Phase.Resolution:
                // this should only run once per turn
                // if(!board.playSequence.ContainsEnemyAction()) {
                //     foreach(EnemyAction actionToAdd in curActions) {
                //         if(!board.playSequence.Contains(actionToAdd)) {
                //             int idx = board.playSequence.IndexOfCompleteTime(actionToAdd.completeTime);
                //             board.playSequence.Insert(idx + 1, actionToAdd); // insert AFTER given index to give player priority in resolution
                //         }
                //     }
                //     // not sure if these two lines work - check later if something is wrong
                //     prevActions = curActions;
                //     curActions.Clear();
                // }
                

                break;
        }
    }
}
