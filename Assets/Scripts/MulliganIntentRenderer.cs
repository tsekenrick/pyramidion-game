using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MulliganIntentRenderer : MonoBehaviour {

    public Sprite[] mulliganIntentIcons;
    private SpriteRenderer sr;


    private int DetermineActionType(List<EnemyAction> actions) {
        bool hitAttack = false, hitDefend = false;
        foreach(EnemyAction action in actions) {
            if(action.actionType == ActionType.Attack && !hitAttack) {
                hitAttack = true;
            } else if(action.actionType == ActionType.Defense && !hitDefend) {
                hitDefend = true;
            }
        }
        if(hitAttack && hitDefend) {
            return 2;
        } else if (hitDefend) {
            return 1;
        } else {
            return 0;
        }
    }

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        transform.DOMoveY(transform.position.y + .2f, .7f).SetLoops(-1, LoopType.Yoyo);
    }

    void Update() {
        if(Board.me.curPhase == Phase.Mulligan) {
            sr.enabled = true;
            sr.sprite = mulliganIntentIcons[DetermineActionType(transform.parent.GetComponent<Enemy>().curActions)];
        } else {
            sr.enabled = false;
        }
    }
}
