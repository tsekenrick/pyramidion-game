using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Enemy : Target
{
    protected Board board;

    public List<EnemyAction> prevActions;
    public List<EnemyAction> curActions;
    public Sprite[] intentIcons;
    public TextMeshPro[] healthText;

    private bool dying;
    public int maxHealth;
    
    protected virtual void Start() {
        dying = false;
        startPos = this.transform.position;
        board = Board.me;
        healthText = GetComponentsInChildren<TextMeshPro>();
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

    private IEnumerator Die() {
        for(int i = board.playSequence.Count - 1; i >= 0; i--) {
            if(board.playSequence[i] is EnemyAction) {
                EnemyAction toRemove = board.playSequence[i] as EnemyAction;
                if(toRemove.owner == this.gameObject) board.playSequence.Remove(board.playSequence[i]);
            }

            // redirect any actions in playSequence that were targetted at this enemy
            else if(board.playSequence[i].target == this.gameObject) {
                if(board.enemies.Length == 1) {
                    board.playSequence.Remove(board.playSequence[i]);
                } else {
                    foreach(GameObject enemy in board.enemies) {
                        if(enemy != this.gameObject) board.playSequence[i].target = enemy;
                    }
                }
            }
        }
        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
        this.transform.DOShakePosition(1f, .50f);
        yield return new WaitForSeconds(.25f);
        DOTween.To(()=> sr.color, x=> sr.color = x, new Color(sr.color.r, sr.color.g, sr.color.b, 0), 1.5f);
        yield return new WaitForSeconds(1.5f);
        Destroy(this.gameObject);
    }

    protected virtual void Update() {
        // healthbar/block overlay logic
        foreach(SpriteRenderer sr in blockOverlay) sr.enabled = (block > 0);
        healthBar.DOScaleX(Mathf.Max(0, (float)health/maxHealth), .3f);
        
        // health text
        healthText[0].text = health > 0 ? $"{health}/{maxHealth}" : $"0/{maxHealth}";
        healthText[1].text = block > 0 ? block.ToString() : " ";

        if(health <= 0 && !dying) {
            dying = true;
            StartCoroutine(Die());
        } 

        // switch(board.curPhase) {
        //     case Phase.Mulligan:
        //         // roll for action type and display non-numerical intent
        //         if(curActions.Count == 0) {
        //             for(int i = 0; i < Random.Range(2, 3); i++) { 
        //                 ActionType actionType = (ActionType)Random.Range(0, 2); // change to max 3 when we added statuses
        //                 // temporary cheeky one-liner - will not work for ActionType.Status
        //                 GameObject target = actionType == ActionType.Attack ? GameObject.Find("Player") : this.gameObject;
        //                 curActions.Add(new EnemyAction(actionType, target, this.gameObject));
        //             }
        //         }
        //         curActions.Sort((a, b) => a.completeTime.CompareTo(b.completeTime));
        //         break;
        // }
    }
}
