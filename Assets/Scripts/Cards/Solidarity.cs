using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Solidarity : Card {
    public int handPos;

    public override void resolveAction() {
        base.resolveAction();
        this.cardProps[1] = "4";
    }

    public override void OnDraw() {
        this.cardProps[1] = "4";
        base.OnDraw();
    }

    public override void Awake() {
        base.Awake();
    }

    public override void Update() {
        base.Update();
        if(curState == CardState.InHand || curState == CardState.InQueue) {
            handPos = int.Parse(transform.parent.name[transform.parent.name.Length - 1].ToString());
        }

        if(handPos == 0) {
            if(GameObject.Find($"Hand{handPos + 1}").GetComponentInChildren<Card>().cardName == "Solidarity") {
                this.cardProps[1] = "6";
            }
        } else if(handPos == 4) {
            if(GameObject.Find($"Hand{handPos - 1}").GetComponentInChildren<Card>().cardName == "Solidarity") {
                this.cardProps[1] = "6";
            }
        } else {
            if(GameObject.Find($"Hand{handPos + 1}").GetComponentInChildren<Card>().cardName == "Solidarity" &&
                GameObject.Find($"Hand{handPos - 1}").GetComponentInChildren<Card>().cardName == "Solidarity") {
                this.cardProps[1] = "8";
            } else if(GameObject.Find($"Hand{handPos + 1}").GetComponentInChildren<Card>().cardName == "Solidarity" ||
                GameObject.Find($"Hand{handPos - 1}").GetComponentInChildren<Card>().cardName == "Solidarity") {
                this.cardProps[1] = "6";
            }
        }

        transform.Find("CardDesc").GetComponent<TextMeshPro>().text =  int.Parse(this.cardProps[1]) > 4 ?
            $"Deal <color=#2bce43>{this.cardProps[1]}</color> damage. Deals 2 more damage for each adjacent copy of Solidarity in your hand." :
            $"Deal 4 damage. Deals 2 more damage for each adjacent copy of Solidarity in your hand.";
          
    }
}
