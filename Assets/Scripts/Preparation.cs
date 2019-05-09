using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

[System.Serializable]
public class Preparation : Card {

    public override void resolveAction() {
        Target t = target.GetComponentInParent<Target>();
        t.transform.Find("DamageText").GetComponent<TextMeshPro>().text = "8";
        t.GetComponentInChildren<DamageText>().FadeText();
        t.health -= 8;

        t.transform.Find("TakingDamagePS").GetComponent<ParticleSystem>().Play();
        Camera.main.transform.DOShakePosition(.5f);
    }
    
    public override void OnMulligan() {
        Debug.Log("called onmulligan for preparation");
        Board.me.player.GetComponent<Player>().block += 3;
        Board.me.player.transform.Find("ShieldPS").GetComponent<ParticleSystem>().Play();
    }

    public override void Awake() {
        base.Awake();
    }

    public override void Update() {
        base.Update();
    }

}
