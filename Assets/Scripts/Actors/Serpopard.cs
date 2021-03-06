﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Serpopard : Enemy {
    protected bool isEnraged;
    public SoundManager sm;

    protected override void Start() {
        base.Start();
        isEnraged = false;
    }

    protected override void Update() {
        base.Update();
        switch(board.curPhase) {
            case Phase.Mulligan:
                if(curActions.Count == 0) {
                    if(health < .3f * maxHealth && !isEnraged) {
                        isEnraged = true;
                        // FMOD Play Enrage Sound
                        sm = SoundManager.me;
                        sm.PlayEnrageSound();
                        GetComponent<SpriteRenderer>().DOColor(new Color(.95f, .45f, .45f, 1f), 1f).SetDelay(2f);
                        StartCoroutine(DelayedChargeParticle());

                        curActions.Add(new EnemyAction(ActionType.Attack, board.player, this.gameObject, 5, 3));
                        curActions.Add(new EnemyAction(ActionType.Attack, board.player, this.gameObject, 5, 5));
                        curActions.Add(new EnemyAction(ActionType.Attack, board.player, this.gameObject, 5, 7));
                        curActions.Add(new EnemyAction(ActionType.Attack, board.player, this.gameObject, 12, 10));
                    } else if(health < .3f * maxHealth && isEnraged) {
                        curActions.Add(new EnemyAction(ActionType.Defense, this.gameObject, this.gameObject, 1, 5));
                    } else {
                        int turnType = Random.Range(0, 4);
                        switch(turnType) {
                            case 0:
                                curActions.Add(new EnemyAction(ActionType.Attack, board.player, this.gameObject, 4, 3));
                                curActions.Add(new EnemyAction(ActionType.Attack, board.player, this.gameObject, 4, 5));
                                break;
                            case 1:
                                curActions.Add(new EnemyAction(ActionType.Defense, this.gameObject, this.gameObject, 10, 5));
                                break;
                            case 2:
                                curActions.Add(new EnemyAction(ActionType.Attack, board.player, this.gameObject, 9, 5));
                                curActions.Add(new EnemyAction(ActionType.Attack, board.player, this.gameObject, 9, 7));
                                break;
                        }
                    }
                }
                curActions.Sort((a, b) => a.completeTime.CompareTo(b.completeTime));
                break;
        }
    }

    private IEnumerator DelayedChargeParticle() {
        yield return new WaitForSeconds(2.4f);
        transform.Find("ChargePS").GetComponent<ParticleSystem>().Play();
    }
}
