using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

[System.Serializable]
public class ShieldBash : Card {

    public override void ResolveAction() {
        Attack(Board.instance.player.GetComponent<Player>().block, target);
        SoundManager.me.PlayPlayerAttackSound();
        charged = false;
    }

    public override void Awake() {
        base.Awake();
    }

    public override void Update() {
        base.Update();
    }

}
