using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TemperedGuard : Card {
    
    public override void ResolveAction() {
        base.ResolveAction();
        this.cardProps[1] = "2";
    }

    public override void OnEnqueue() {
        int thisIdx = Board.instance.playSequence.IndexOf(this.action);
        if(thisIdx == 0) {
            return;
        } else {
            PlayerAction action = Board.instance.playSequence[thisIdx - 1] as PlayerAction;
            if(action.card.cardName == "Tempered Guard") {
                this.cardProps[1] = (int.Parse(action.card.cardProps[1]) + 4).ToString();
                Debug.Log($"tempered guard assigned for {int.Parse(action.card.cardProps[1]) + 4} block");
            }
        }
    }

    public override void OnDequeue() {
        this.cardProps[1] = "2";
    }

    public override void Awake() {
        base.Awake();
        alias = "T.G.";
    }

    public override void Update() {
        base.Update();
    }

}
