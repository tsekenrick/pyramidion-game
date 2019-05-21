using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

[System.Serializable]
public class Solidarity : Card {
    public int handPos;

    public override void ResolveAction() {
        SoundManager sm = SoundManager.me;
        sm.PlayPlayerAttackSound();
        int amount = charged ? 14 : 7;
        foreach(GameObject enemy in Board.instance.enemies) {
            Attack(5, enemy);
            int tmpBlock = enemy.GetComponent<Target>().block;
            if(Mathf.Max(amount - tmpBlock, 0) > 0) {
                enemy.transform.Find("TakingDamagePS").GetComponent<ParticleSystem>().Play();
            } else {
                enemy.transform.Find("DamagedShieldPS").GetComponent<ParticleSystem>().Play();
            }
            
        }
        // do the screenshake only once
        if(Mathf.Max(amount - Board.instance.enemies[0].GetComponent<Target>().block, 0) > 0) {
            Camera.main.transform.DOShakePosition(.5f);
        } else {
            Camera.main.transform.DOShakePosition(.5f, .5f);
        }

        charged = false;
        Board.instance.player.GetComponent<Target>().block += int.Parse(this.cardProps[1]);
        if(int.Parse(this.cardProps[1]) > 0) Board.instance.player.transform.Find("ShieldPS").GetComponent<ParticleSystem>().Play();
        this.cardProps[1] = "0";
    }

    public override void OnDraw() {
        this.cardProps[1] = "0";
        base.OnDraw();
    }

    public override void Awake() {
        base.Awake();
        shake = false;
        transform.Find("CardDesc").GetComponent<TextMeshPro>().enabled = false;
    }

    public override void Update() {
        base.Update();
        if(curState == CardState.InHand || curState == CardState.InQueue) {
            handPos = int.Parse(transform.parent.name[transform.parent.name.Length - 1].ToString());
        }

        if(Board.instance.curPhase == Phase.Mulligan && Board.instance.hand.Count == 5) {
            if(handPos == 0) {
                if(GameObject.Find($"Hand{handPos + 1}").GetComponentInChildren<Card>().cardName == "Solidarity") {
                    this.cardProps[1] = "3";
                } else {
                    this.cardProps[1] = "0";
                }
            } else if(handPos == 4) {
                if(GameObject.Find($"Hand{handPos - 1}").GetComponentInChildren<Card>().cardName == "Solidarity") {
                    this.cardProps[1] = "3";
                } else {
                    this.cardProps[1] = "0";
                }
            } else {
                if(GameObject.Find($"Hand{handPos + 1}").GetComponentInChildren<Card>().cardName == "Solidarity" &&
                    GameObject.Find($"Hand{handPos - 1}").GetComponentInChildren<Card>().cardName == "Solidarity") {
                    this.cardProps[1] = "6";
                } else if(GameObject.Find($"Hand{handPos + 1}").GetComponentInChildren<Card>().cardName == "Solidarity" ||
                    GameObject.Find($"Hand{handPos - 1}").GetComponentInChildren<Card>().cardName == "Solidarity") {
                    this.cardProps[1] = "3";
                } else {
                    this.cardProps[1] = "0";
                }
            }
        }

        transform.Find("CardDesc").GetComponent<TextMeshPro>().text =  int.Parse(this.cardProps[1]) > 0 ?
            $"Deal 7 damage to ALL enemies. Gain 3 <color=#2bce43>({this.cardProps[1]})</color> block for each adjacent copy of Solidarity in your hand." :
            $"Deal 7 damage to ALL enemies. Gain 3 block for each adjacent copy of Solidarity in your hand.";

    }
}
