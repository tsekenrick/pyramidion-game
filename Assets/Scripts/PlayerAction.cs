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

    // move to `Card.cs` so that superclasses of `Card` can override it? - make sure to change `ExecuteAction` coroutine as well
    // public void resolveAction() {
    //     MethodInfo mi = this.card.GetType().GetMethod(this.card.cardProps[0]);
    //     switch(card.cardProps[0]) {
    //         case "Attack":
    //             // FMOD Player Attack Sound
    //             sm = SoundManager.me;
    //             sm.PlayPlayerAttackSound();
    //             break;
    //         case "Defend":
    //             // FMOD Player Defend Sound
    //             sm = SoundManager.me;


    //             sm.PlayPlayerDefendSound();
    //             break;
    //     }
    //     mi.Invoke(this.card, new object[]{int.Parse(this.card.cardProps[1]), this.target});
    // }

}