using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SandCloak : Card {

    public override void ResolveAction() {
        base.ResolveAction();
        int value = Mathf.Max(int.Parse(this.cardProps[1]) - 4, 0);
        this.cardProps[1] = value.ToString();
    }

    public override void Awake() {
        base.Awake();
    }

    public override void OnNewCombat() {
        this.cardProps[1] = "18";
        base.OnNewCombat();
    }

    public override void Update() {
        base.Update();

        this.desc = int.Parse(this.cardProps[1]) < 18 ? 
            $"Gain <color=#901C09>{this.cardProps[1]}</color> block. Decrease this card's block by 4 this combat." : 
            "Gain 18 block. Decrease this card's block by 4 this combat.";
    }

}
