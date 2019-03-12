using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ProgressBar : MonoBehaviour
{
    private Board board;

    void Start() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);
        board = Board.me;
    }

    void Update() {
        if(board.curPhase == Phase.Play || board.curPhase == Phase.Resolution) {
            transform.DOScaleX(board.playSequence.totalTime, .2f * board.playSequence.totalTime);
        }
    }
}
