using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DarkProgressBar : MonoBehaviour {

    private Board board;
    private SpriteRenderer sr;
    private float barLength;
    private List<EnemyAction> enemyActions;

    public IEnumerator AdjustForBorrowedTime() {
        GameObject.Find("Actions").GetComponent<ActionRenderer>().adjusted = true;
        enemyActions = new List<EnemyAction>();
        foreach(GameObject enemy in board.enemies) {
            foreach(EnemyAction action in enemy.GetComponent<Enemy>().curActions) {
                enemyActions.Add(action);
            }
        }
        enemyActions.Sort((a, b) => a.completeTime.CompareTo(b.completeTime));
        barLength = (15 - enemyActions[enemyActions.Count - 1].baseCompleteTime) * 1.03f;
        transform.localScale = new Vector3(barLength, transform.localScale.y, transform.localScale.z);
        yield return new WaitForSeconds(1.5f);
        barLength = (15 - enemyActions[enemyActions.Count - 1].completeTime) * 1.03f;
        transform.DOScaleX(barLength, .2f);
    }

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        board = Board.me;
    }

    void Update() {
        sr.enabled = board.curPhase == Phase.Play;
        if(board.curPhase == Phase.Play) {
            // enemyActions = new List<EnemyAction>();
            // foreach(GameObject enemy in board.enemies) {
            //     foreach(EnemyAction action in enemy.GetComponent<Enemy>().curActions) {
            //         enemyActions.Add(action);
            //     }
            // }
            // enemyActions.Sort((a, b) => a.completeTime.CompareTo(b.completeTime));
            
            if(board.borrowedTime == 0) {
                barLength = (15 - enemyActions[enemyActions.Count - 1].completeTime) * 1.03f;
                transform.localScale = new Vector3(barLength, transform.localScale.y, transform.localScale.z);
            }
            
        }
        
    }
}
