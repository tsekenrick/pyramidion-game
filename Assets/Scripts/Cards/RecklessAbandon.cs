using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecklessAbandon : Card {

    public override void resolveAction() {
        Card.charged = true;
        Board.me.player.transform.Find("ChargePS").GetComponent<ParticleSystem>().Play();
        Board.me.player.GetComponent<SpriteRenderer>().sprite = Board.me.player.GetComponent<Player>().combatStates[2];
    }

    public override void Awake() {
        base.Awake();
    }

    public override void Update() {
        base.Update();
    }
}
