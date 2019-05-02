using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CalculatedStrike : Card {

    public override void resolveAction() {
        base.resolveAction();
        this.cardProps[1] = "3";
        Debug.Log("resolved in superclass");
    }
    
    public override void OnMulligan() {
        Debug.Log("called onmulligan for calc strike");
        this.cardProps[1] = (int.Parse(this.cardProps[1]) + 5).ToString();
    }

    public override void Awake() {
        base.Awake();
    }

    public override void Update() {
        base.Update();

        this.desc = int.Parse(this.cardProps[1]) > 3 ? 
            $"Deal <color=#2bce43>{this.cardProps[1]}</color> damage plus 5 for every time this card is mulliganed until it is played." : 
            "Deal 3 damage plus 5 for every time this card is mulliganed until it is played.";
    }

}
