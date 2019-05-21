using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CalculatedStrike : Card {

    public override void ResolveAction() {
        base.ResolveAction();
        this.cardProps[1] = "3";
    }
    
    public override void OnMulligan() {
        this.cardProps[1] = (int.Parse(this.cardProps[1]) + 8).ToString();
    }

    public override void Awake() {
        base.Awake();
    }

    public override void OnNewCombat() {
        this.cardProps[1] = "3";
        base.OnNewCombat();
    }

    public override void Update() {
        base.Update();

        this.desc = int.Parse(this.cardProps[1]) > 3 ? 
            $"Deal <color=#2bce43>{this.cardProps[1]}</color> damage. Increase this card's damage by 8 when it is mulliganed. Resets on use." :
            "Deal 3 damage. Increase this card's damage by 8 when it is mulliganed. Resets on use.";
    }

}
