using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction: Action 
{
    public Card card;

    public PlayerAction(Card card, GameObject target) : base(target) {
        this.card = card;
    }

    public void resolveAction() {
        // do things to resolve action based on data given in card field
    }

}