using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FocusAttack : Card {

    public override void resolveAction() {
        if(Board.me.playSequence.ContainsActionAtTime(this.action.completeTime - 1) ||
           Board.me.playSequence.ContainsActionAtTime(this.action.completeTime - 2) ||
           Board.me.playSequence.ContainsActionAtTime(this.action.completeTime - 3)) {
            this.cardProps[1] = "10";
        } else {
            this.cardProps[1] = "20";
            Debug.Log("focus attack satisfied conditions at resolution");
        }
        base.resolveAction();
        this.cardProps[1] = "10";
    }

    public override void Awake() {
        base.Awake();
    }

    public override void Update() {
        base.Update();
    }
}
