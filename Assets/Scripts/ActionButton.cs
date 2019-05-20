using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    private Board board;
    public Sprite[] mulliganButtons;
    public Sprite[] executeButtons;
    private SpriteRenderer sr;
    private SpriteRenderer glow;
    private CircleCollider2D col;
    public bool buttonPressed;
    private bool renderPressed;
    public bool canClick;
    private float glowAlpha;

    // FMOD variables
    [FMODUnity.EventRef]
    public string actionButtonDownSoundEvent;
    [FMODUnity.EventRef]
    public string actionButtonUpSoundEvent;

    FMOD.Studio.EventInstance actionButtonDownSound;
    FMOD.Studio.EventInstance actionButtonUpSound;

    private void Awake()
    {
        // FMOD object init
        actionButtonDownSound = FMODUnity.RuntimeManager.CreateInstance(actionButtonDownSoundEvent);
        actionButtonUpSound = FMODUnity.RuntimeManager.CreateInstance(actionButtonUpSoundEvent);
    }

    void Start() {
        buttonPressed = false;
        canClick = true;
        board = Board.instance;
        glowAlpha = .55f;
        col = this.GetComponent<CircleCollider2D>();
        sr = this.GetComponent<SpriteRenderer>();
        glow = GameObject.Find("ActionBtnGlow").GetComponent<SpriteRenderer>();
    }

    void Update() {
        glow.enabled = (board.curPhase != Phase.Resolution);

        switch(board.curPhase) {
            case Phase.Mulligan:
                int idxOffset = board.toMul.Count > 0 ? 0 : 2;
                glow.color = board.toMul.Count > 0 ? new Color(0, .6f, .85f, glowAlpha) : new Color(0, .85f, .3f, glowAlpha);
                sr.sprite = renderPressed ? mulliganButtons[1 + idxOffset] : mulliganButtons[0 + idxOffset];
                break;

            case Phase.Play:
                glow.color = new Color(0, .6f, .25f, glowAlpha);
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
        actionButtonDownSound.start();
    }

    public void OnMouseEnter() {
        if(board.overlayActive) return;
        // FMOD Play Pile Hover Sound     
        SoundManager sm = SoundManager.me;
        sm.PlaySound(sm.pileHoverSound);
        glowAlpha = 1f;
    }

    public void OnMouseExit() {
        if(board.overlayActive) return;
        glowAlpha = .55f;

        if(renderPressed) actionButtonUpSound.start();
        renderPressed &= false;
    }

    public void OnMouseUpAsButton() {
        if(board.overlayActive || !canClick) return;
        StartCoroutine(SpamDisabler());
        
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
        actionButtonUpSound.start();
    }

    private IEnumerator SpamDisabler() {
        canClick = false;
        yield return new WaitForSeconds(.7f);
        canClick = true;
    }
}
