using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

[System.Serializable]
public class Preparation : Card {

    public override void ResolveAction() {
        Target t = target.GetComponentInParent<Target>();
        int damage = charged ? 24 : 12;
        t.transform.Find("DamageText").GetComponent<TextMeshPro>().text = $"{damage}";
        t.GetComponentInChildren<DamageText>().FadeText();
        t.health -= damage;

        t.transform.Find("TakingDamagePS").GetComponent<ParticleSystem>().Play();
        Camera.main.transform.DOShakePosition(.5f);
        charged = false;
        SoundManager.me.PlayPlayerAttackSound();
    }
    
    public override void OnMulligan() {
        Debug.Log("called onmulligan for preparation");
        Board.instance.player.GetComponent<Player>().block += 2;
        Board.instance.player.transform.Find("ShieldPS").GetComponent<ParticleSystem>().Play();
        SoundManager.me.PlayPlayerDefendSound();
    }

    public override void Awake() {
        base.Awake();
    }

    public override void Update() {
        base.Update();
    }

}
