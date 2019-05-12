using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FocusAttack : Card {

    public override void resolveAction() {
        if(Board.me.prevResolvedAction == "PlayerAction") {
            this.cardProps[1] = "20";
            Debug.Log("focus attack satisfied conditions at resolution");
        } else {
            this.cardProps[1] = "10";
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
