using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlayerAction: Action 
{
    public Card card;

    public PlayerAction(Card card, GameObject target) : base(target) {
        this.card = card;
    }

    public void resolveAction() {
        MethodInfo mi = this.card.GetType().GetMethod(this.card.cardProps[0]);
        switch(card.cardProps[0]) {
            case "Attack":
                // play attack sound
                break;
            case "Defend":
                // play defend sound
                break;
        }
        mi.Invoke(this.card, new object[]{int.Parse(this.card.cardProps[1]), this.target});
        // do things to resolve action based on data given in card field
    }

}