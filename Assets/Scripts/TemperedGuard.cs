﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TemperedGuard : Card {
    
    public override void resolveAction() {
        base.resolveAction();
        this.cardProps[1] = "3";
    }

    public override void OnEnqueue() {
        int thisIdx = Board.me.playSequence.IndexOf(this.action);
        if(thisIdx == 0) {
            return;
        } else {
            PlayerAction action = Board.me.playSequence[thisIdx - 1] as PlayerAction;
            if(action.card.cardName == "Tempered Guard") {
                this.cardProps[1] = (int.Parse(action.card.cardProps[1]) + 3).ToString();
                Debug.Log($"tempered guard assigned for {int.Parse(action.card.cardProps[1]) + 3} block");
            }
        }
    }

    public override void OnDequeue() {
        this.cardProps[1] = "3";
    }

    public override void Awake() {
        base.Awake();
    }

    public override void Update() {
        base.Update();
    }

}
