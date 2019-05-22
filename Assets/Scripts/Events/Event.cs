using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Event : MonoBehaviour {

    public Sprite[] eventStates;
    private TextMeshPro[] tmps;
    private SpriteRenderer sr;
    protected Board board;

    // FMOD variables
    private SoundManager sm = SoundManager.me;

    // do the thing the event says it will do, and then set game back to mul phase
    protected virtual void ResolveEvent() {
        StartCoroutine(board.EventToMulPhase());
    }

    public virtual void ResolveConfirm() {
        return;
    }

    protected virtual void Start() {
        sr = this.GetComponent<SpriteRenderer>();
        tmps = this.GetComponentsInChildren<TextMeshPro>();
        board = Board.instance;
        Color initColor = sr.color;
        foreach(TextMeshPro tmp in tmps) {
            Color tmpColor = tmp.color;
            tmp.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, 0f);
            tmp.DOColor(tmpColor, .5f);

        }
        sr.color = new Color(initColor.r, initColor.g, initColor.b, 0f);
        sr.DOColor(initColor, .5f);
    }

    void OnMouseEnter() {
        if(DeckDisplay.instance.isRendering) return;
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
        ResolveEvent();
    }
}
