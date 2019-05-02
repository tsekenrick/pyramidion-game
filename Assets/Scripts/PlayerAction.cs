using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlayerAction: Action 
{
    public Card card;
    public SoundManager sm;
    public PlayerAction(Card card, GameObject target) : base(target) {
        this.card = card;
    }
}