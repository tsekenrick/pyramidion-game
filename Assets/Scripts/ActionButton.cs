﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour {

    private Board board;
    public Sprite[] mulliganButtons;
    public Sprite[] executeButtons;
    public Sprite[] redrawNumbers;
    public Sprite[] redrawGlows;
    private SpriteRenderer sr;
    private SpriteRenderer glow;
    private SpriteRenderer glow2;
    private SpriteRenderer counterNum;
    private SpriteRenderer counterGlow;
    private SpriteRenderer redrawText;
    private SpriteRenderer redrawIcon;
    private CircleCollider2D col;
    private float glowAlpha;

    public bool buttonPressed;
    private bool renderPressed;
    public bool canClick;

    public SoundManager sm;

    void Start() {
        sm = SoundManager.me;
        buttonPressed = false;
        canClick = true;
        board = Board.instance;
        glowAlpha = .55f;
        col = this.GetComponent<CircleCollider2D>();
        sr = this.GetComponent<SpriteRenderer>();
        glow = transform.parent.Find("ActionBtnGlow").GetComponent<SpriteRenderer>();
        glow2 = transform.parent.Find("ActionBtnGlow2").GetComponent<SpriteRenderer>();
        counterNum = transform.parent.Find("RedrawNumber").GetComponent<SpriteRenderer>();
        counterGlow = counterNum.transform.Find("RedrawGlow").GetComponent<SpriteRenderer>();
        redrawText = transform.parent.Find("RedrawText").GetComponent<SpriteRenderer>();
        redrawIcon = transform.parent.Find("RedrawIcon").GetComponent<SpriteRenderer>();
    }

    void Update() {
        glow.enabled = (board.curPhase != Phase.Resolution);
        counterGlow.enabled = board.curPhase == Phase.Mulligan;
        counterNum.enabled = board.curPhase == Phase.Mulligan;
        redrawIcon.enabled = board.curPhase == Phase.Mulligan;
        redrawText.enabled = board.curPhase == Phase.Mulligan;
        
        switch(board.curPhase) {
            case Phase.Mulligan:
                int idxOffset = board.toMul.Count > 0 ? 0 : 2;
                glow.color = board.toMul.Count > 0 ? new Color(0.15f, .71f, .95f, glowAlpha) : new Color(0.45f, 0.9f, 0.45f, glowAlpha);
                glow2.color = board.toMul.Count > 0 ? new Color(0.15f, .71f, .95f, glowAlpha) : new Color(0.45f, 0.9f, 0.45f, glowAlpha);
                sr.sprite = renderPressed ? mulliganButtons[1 + idxOffset] : mulliganButtons[0 + idxOffset];
                if(board.mulLimit > 7) {
                    Debug.Log($"known mul limit exceeded at {board.mulLimit}");
                    counterGlow.sprite = redrawGlows[0];
                    counterNum.sprite = redrawNumbers[0];
                } else {
                    counterGlow.sprite = redrawGlows[board.mulLimit];
                    counterNum.sprite = redrawNumbers[board.mulLimit];  
                }
                
                break;

            case Phase.Play:
                glow.color = new Color(0, .6f, .25f, glowAlpha);
                glow2.color = new Color(0, .6f, .25f, glowAlpha);
                sr.sprite = renderPressed ? executeButtons[1] : executeButtons[0];
                break;

            case Phase.Resolution:
                sr.sprite = executeButtons[0];
                break;

        }
    }

    public void OnMouseDown() {
        if(board.overlayActive || !canClick) return;
        renderPressed = true;

        // FMOD Action Button Down Sound Event
        sm.PlaySound(sm.actionButtonDownSound);
    }

    public void OnMouseEnter() {
        if(board.overlayActive) return;
        glowAlpha = 1f;

        // FMOD Action Button Hover Sound     
        sm.PlaySound(sm.actionButtonHoverSound);
    }

    public void OnMouseExit() {
        if(board.overlayActive) return;
        glowAlpha = .55f;

        if(renderPressed) sm.PlaySound(sm.actionButtonUpSound);
        renderPressed &= false;
    }

    public void OnMouseUpAsButton() {
        if(board.overlayActive || !canClick) return;
        
        switch(board.curPhase){
            case Phase.Mulligan:
                buttonPressed = true;
                renderPressed = false;
                break;
            
            case Phase.Play:
                buttonPressed = true;
                renderPressed = false;
                break;
        }
        // FMOD Action Button Up Sound Event
        sm.PlaySound(sm.actionButtonUpSound);

        StartCoroutine(SpamDisabler());
    }

    private IEnumerator SpamDisabler() {
        canClick = false;
        yield return new WaitForSeconds(.6f);
        canClick = true;
    }
}
