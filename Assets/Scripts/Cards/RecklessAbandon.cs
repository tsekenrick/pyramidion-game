using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecklessAbandon : Card {
    // connect to FMOD sound manager
    private SoundManager sm;

    public override void ResolveAction() {
        // FMOD play player buff sound
        sm = SoundManager.me;
        sm.PlayPlayerBuffSound();

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
