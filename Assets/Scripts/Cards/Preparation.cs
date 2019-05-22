using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preparation : Card {

    public override void ResolveAction() {
        Board.instance.player.GetComponent<SpriteRenderer>().sprite = Board.instance.player.GetComponent<Player>().combatStates[2];
        SoundManager sm = SoundManager.me;
        sm.PlayPlayerDefendSound();
        Board.instance.player.transform.Find("DefendBuffPS").GetComponent<ParticleSystem>().Play();
        charged = false;
        prepared++;
    }

    public override void Awake() {
        base.Awake();
        alias = "Prep.";
    }

    public override void Update() {
        base.Update();
    }
}
