using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Anubis : Enemy {
    private int roundsSinceSpawn;
    public int addCount;
    public GameObject add;

    protected override void Start() {
        base.Start();
        roundsSinceSpawn = -1;
    }

    protected override void Update() {
        base.Update();
        switch(board.curPhase) {
            case Phase.Mulligan:
                if(curActions.Count == 0) {
                    if(board.round == 0) {
                        curActions.Add(new EnemyAction(ActionType.Defense, this.gameObject, this.gameObject, 5, 4));
                        curActions.Add(new EnemyAction(ActionType.Attack, board.player, this.gameObject, 8, 6));
                    } else {
                        switch(roundsSinceSpawn) {
                            case -1:
                                // recovery turn
                                curActions.Add(new EnemyAction(ActionType.Defense, this.gameObject, this.gameObject, 8, 8));
                                break;
                            case 0:
                                // base turn
                                DoBaseTurn(new float[3] {.5f, .8f, 1f});
                                break;
                            case 1:
                                // weighted for spawn
                                DoBaseTurn(new float[3] {.3f, .5f, 1f});
                                break;
                            case 2:
                                // spawn
                                curActions.Add(new EnemyAction(ActionType.Summon, this.gameObject, this.gameObject, 1, 5));
                                roundsSinceSpawn = -2;
                                break;
                        }
                    } 
                    roundsSinceSpawn++;
                }
                curActions.Sort((a, b) => a.completeTime.CompareTo(b.completeTime));
                break;
        }
    }

    private void DoBaseTurn(float[] weights) {
        float selector = Random.Range(0f, 1f);
        // prevent more spawning if there are two adds already
        if(board.enemies.Length >= 3) weights = new float[] {.5f, 1f, 1f};

        if(selector <= weights[0]) {
            curActions.Add(new EnemyAction(ActionType.Attack, board.player, this.gameObject, 20, 8));
        } else if (selector <= weights[1]) {
            curActions.Add(new EnemyAction(ActionType.Defense, this.gameObject, this.gameObject, 12, 5));
        } else {
            curActions.Add(new EnemyAction(ActionType.Summon, this.gameObject, this.gameObject, 1, 5));
            roundsSinceSpawn = -2; // turns into -1 due to auto increment at end of action generation
        }   
    }

    public void SummonMedjed() {
        addCount++;
        GameObject newAdd = Instantiate(add, transform.parent, false);
        newAdd.GetComponent<Enemy>().health = newAdd.GetComponent<Enemy>().maxHealth;
        SpriteRenderer sr = newAdd.GetComponent<SpriteRenderer>();
        Color initColor = sr.color;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
        sr.DOColor(initColor, 2f);
        newAdd.transform.DOLocalMoveX(-4.5f - (3.75f * addCount), .5f - (.05f * addCount));
    }

    public override IEnumerator Die() {
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

        foreach(TooltipBehavior tb in GameObject.FindObjectsOfType<TooltipBehavior>()) {
            Destroy(tb.gameObject);
        }
        
        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
        this.transform.DOShakePosition(1f, .50f);
        
        yield return new WaitForSeconds(.25f);
        DOTween.To(()=> sr.color, x=> sr.color = x, new Color(sr.color.r, sr.color.g, sr.color.b, 0), 1.5f);
        foreach(GameObject enemy in board.enemies) {
            if(enemy != this.gameObject) StartCoroutine(enemy.GetComponent<Enemy>().Die());
        }
        
        yield return new WaitForSeconds(1.5f);
        GetComponent<SpriteRenderer>().enabled = false;
        transform.Find("EnemyHealthBarBase").GetComponent<SpriteRenderer>().enabled = false;
        transform.Find("BasicShadow").GetComponent<SpriteRenderer>().enabled = false;
        yield return StartCoroutine(board.DisplayWinScreen());
        Destroy(this.gameObject);        
    }
}
