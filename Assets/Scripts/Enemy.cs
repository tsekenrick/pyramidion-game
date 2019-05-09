using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Enemy : Target
{
    private Board board;

    public List<EnemyAction> prevActions;
    public List<EnemyAction> curActions;
    public TextMeshPro[] healthText;

    private bool dying;
    public const int MAX_HEALTH = 100;
    
    void Start() {
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
        }
        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
        this.transform.DOShakePosition(1f, .50f);
        yield return new WaitForSeconds(.25f);
        DOTween.To(()=> sr.color, x=> sr.color = x, new Color(sr.color.r, sr.color.g, sr.color.b, 0), 1.5f);
        yield return new WaitForSeconds(1.5f);
        Destroy(this.gameObject);
    }

    void Update() {
        // healthbar/block overlay logic
        foreach(SpriteRenderer sr in blockOverlay) sr.enabled = (block > 0);
        healthBar.DOScaleX(Mathf.Max(0, (float)health/MAX_HEALTH), .3f);
        
        // health text
        healthText[0].text = $"{health}/{MAX_HEALTH}";
        healthText[1].text = block > 0 ? block.ToString() : " ";

        if(health <= 0 && !dying) {
            dying = true;
            StartCoroutine(Die());
        } 

        switch(board.curPhase) {
            case Phase.Mulligan:
                // roll for action type and display non-numerical intent
                if(curActions.Count == 0) {
                    for(int i = 0; i < Random.Range(2, 3); i++) { 
                        ActionType actionType = (ActionType)Random.Range(0, 2); // change to max 3 when we added statuses
                        // temporary cheeky one-liner - will not work for ActionType.Status
                        GameObject target = actionType == ActionType.Attack ? GameObject.Find("Player") : this.gameObject;
                        curActions.Add(new EnemyAction(actionType, target, this.gameObject));
                    }
                }
                curActions.Sort((a, b) => a.completeTime.CompareTo(b.completeTime));
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
