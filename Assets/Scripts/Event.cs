﻿using System.Collections;
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
