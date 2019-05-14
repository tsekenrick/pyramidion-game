using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecklessAbandon : Card {

    public override void ResolveAction() {
        Card.charged = true;
        Board.instance.player.transform.Find("ChargePS").GetComponent<ParticleSystem>().Play();
        Board.instance.player.GetComponent<SpriteRenderer>().sprite = Board.instance.player.GetComponent<Player>().combatStates[2];
    }

    public override void Awake() {
        base.Awake();
    }

    public override void Update() {
        base.Update();
    }
}
