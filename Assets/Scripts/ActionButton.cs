using System.Collections;
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

    // FMOD variables
    [FMODUnity.EventRef]
    public string actionButtonDownSoundEvent;
    [FMODUnity.EventRef]
    public string actionButtonUpSoundEvent;

    FMOD.Studio.EventInstance actionButtonDownSound;
    FMOD.Studio.EventInstance actionButtonUpSound;

    private void Awake() {
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
                glow.color = board.toMul.Count > 0 ? new Color(0.15f, .71f, .95f, glowAlpha) : new Color(0.2f, 0.8f, 0.2f, glowAlpha);
                glow2.color = board.toMul.Count > 0 ? new Color(0.15f, .71f, .95f, glowAlpha) : new Color(0.2f, 0.8f, 0.2f, glowAlpha);
                sr.sprite = renderPressed ? mulliganButtons[1 + idxOffset] : mulliganButtons[0 + idxOffset];
                if(board.mulLimit > 5) {
                    Debug.Log(board.mulLimit);
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
        actionButtonDownSound.start();
    }

    public void OnMouseEnter() {
        if(board.overlayActive) return;
        glowAlpha = 1f;

        // FMOD Play Pile Hover Sound     
        SoundManager sm = SoundManager.me;
        sm.PlaySound(sm.pileHoverSound);
    }

    public void OnMouseExit() {
        if(board.overlayActive) return;
        glowAlpha = .55f;

        if(renderPressed) actionButtonUpSound.start();
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
        actionButtonUpSound.start();

        StartCoroutine(SpamDisabler());
    }

    private IEnumerator SpamDisabler() {
        canClick = false;
        yield return new WaitForSeconds(.6f);
        canClick = true;
    }
}
