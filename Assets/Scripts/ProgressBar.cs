using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ProgressBar : MonoBehaviour
{
    private Board board;
    public int barLength;
    private SpriteRenderer sr;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(0, 1, 0, .5f);
        board = Board.me;
    }

    void Update() {
        transform.DOScaleX(board.playSequence.totalTime * 1.03f, .2f + (.05f * board.playSequence.totalTime));
        int finalEnemyActionTime = 0;
        List<int> enemyActionTimes = new List<int>();
        foreach(GameObject go in board.enemies) {
            Enemy enemy = go.GetComponent<Enemy>();
            foreach(Action action in enemy.curActions) enemyActionTimes.Add(action.completeTime);
        }
        foreach(int time in enemyActionTimes) {
            if(time > finalEnemyActionTime) finalEnemyActionTime = time;
        }
        sr.color = board.playSequence.totalTime > finalEnemyActionTime && board.curPhase == Phase.Play ? new Color(1, 0, 0, .5f) : new Color(0, 1, 0, .5f);
    }
}
