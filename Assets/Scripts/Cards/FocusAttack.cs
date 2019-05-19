using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FocusAttack : Card {

    public override void ResolveAction() {
        if(Board.instance.prevResolvedAction == "PlayerAction") {
            this.cardProps[1] = "24";
        } else {
            this.cardProps[1] = "12";
        }
        base.ResolveAction();
        this.cardProps[1] = "12";
    }

    public override void Awake() {
        base.Awake();
    }

    public override void Update() {
        base.Update();
    }
}
