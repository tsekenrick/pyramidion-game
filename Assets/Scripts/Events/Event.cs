using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour {

    public Sprite[] eventStates;
    private SpriteRenderer sr;
    protected Board board;

    // do the thing the event says it will do, and then set game back to mul phase
    protected virtual void resolveEvent() {

        StartCoroutine(board.EventToMulPhase());
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
