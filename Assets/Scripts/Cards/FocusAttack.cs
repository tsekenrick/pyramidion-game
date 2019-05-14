using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FocusAttack : Card {

    public override void ResolveAction() {
        if(Board.instance.prevResolvedAction == "PlayerAction") {
            this.cardProps[1] = "20";
        } else {
            this.cardProps[1] = "10";
        }
        base.ResolveAction();
        this.cardProps[1] = "10";
    }

    public override void Awake() {
        base.Awake();
    }

    public override void Update() {
        base.Update();
    }
}
