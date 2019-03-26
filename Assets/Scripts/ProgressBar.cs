using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ProgressBar : MonoBehaviour
{
    private Board board;
    public int barLength;

    void Start() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);
        board = Board.me;
    }

    void Update() {
        transform.DOScaleX(board.playSequence.totalTime * 1.03f, .2f + (.05f * board.playSequence.totalTime));
    }
}
