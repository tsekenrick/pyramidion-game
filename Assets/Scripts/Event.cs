using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour {

    public Sprite[] eventStates;
    private SpriteRenderer sr;
    private Board board;

    // do the thing the event says it will do, and then set game back to mul phase
    protected virtual void resolveEvent() {

        // disable dark overlay
        GameObject.Find("_DarknessOverlay").GetComponent<SpriteRenderer>().enabled = false;

        // show mulligan banner
        GameObject phaseBanner = GameObject.Find("PhaseBanner"); 
        phaseBanner.GetComponent<PhaseBanner>().phaseName.text = "Mulligan Phase"; 
        phaseBanner.GetComponent<PhaseBanner>().canBanner = true;
        phaseBanner.GetComponent<PhaseBanner>().doBanner();

        // reset state variables
        Board.me.curPhase = Phase.Mulligan;
        board.mulLimit = 4;
        board.turn = 0;
        board.borrowedTime = 0;
        board.round = 0;
        board.Reshuffle();
        board.level++;

        // spawn new enemies
        GameObject enemySpawner = GameObject.Find("EnemySpawner");
        EnemySpawner spawner = enemySpawner.GetComponent<EnemySpawner>();
        if(board.level != 4) {
            for(int i = 0; i < board.level; i++) {
                Instantiate(spawner.enemyList[Random.Range(0, spawner.enemyList.Length)], new Vector3(i * -3f, 0, 9.3f), Quaternion.identity, enemySpawner.transform);
            }
        } else {
            Instantiate(spawner.boss, new Vector3(0, 0, 9.3f), Quaternion.identity, enemySpawner.transform);
        }
        
        return;
    }

    protected virtual void Start() {
        sr = this.GetComponent<SpriteRenderer>();
        board = Board.me;
    }

    void OnMouseEnter() {
        sr.sprite = eventStates[1];
    }

    void OnMouseExit() {
        sr.sprite = eventStates[0];
    }

    void OnMouseUpAsButton() {
        resolveEvent();
    }
}
