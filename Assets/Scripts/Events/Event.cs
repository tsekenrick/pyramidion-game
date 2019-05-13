using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour {

    public Sprite[] eventStates;
    private SpriteRenderer sr;
    protected Board board;

    // FMOD variables
    private SoundManager sm = SoundManager.me;

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

        // FMOD Play Hover Event Sound
        sm.PlaySound(sm.hoverEventButtonSound);
    }

    void OnMouseExit() {
        sr.sprite = eventStates[0];
    }

    void OnMouseUpAsButton() {
        // FMOD Play Click Event Sound
        sm.PlaySound(sm.clickEventButtonSound);

        resolveEvent();
    }
}
