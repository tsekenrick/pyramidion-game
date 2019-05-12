using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Solidarity : Card {
    public int handPos;

    public override void resolveAction() {
        base.resolveAction();
        this.cardProps[1] = "5";
    }

    public override void OnDraw() {
        this.cardProps[1] = "5";
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
                this.cardProps[1] = "8";
            }
        } else if(handPos == 4) {
            if(GameObject.Find($"Hand{handPos - 1}").GetComponentInChildren<Card>().cardName == "Solidarity") {
                this.cardProps[1] = "8";
            }
        } else {
            if(GameObject.Find($"Hand{handPos + 1}").GetComponentInChildren<Card>().cardName == "Solidarity" &&
                GameObject.Find($"Hand{handPos - 1}").GetComponentInChildren<Card>().cardName == "Solidarity") {
                this.cardProps[1] = "11";
            } else if(GameObject.Find($"Hand{handPos + 1}").GetComponentInChildren<Card>().cardName == "Solidarity" ||
                GameObject.Find($"Hand{handPos - 1}").GetComponentInChildren<Card>().cardName == "Solidarity") {
                this.cardProps[1] = "8";
            }
        }

        transform.Find("CardDesc").GetComponent<TextMeshPro>().text =  int.Parse(this.cardProps[1]) > 5 ?
            $"Deal <color=#2bce43>{this.cardProps[1]}</color> damage. Deals 3 more damage for each adjacent copy of Solidarity in your hand." :
            $"Deal 5 damage. Deals 3 more damage for each adjacent copy of Solidarity in your hand.";
          
    }
}
