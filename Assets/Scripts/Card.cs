﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum CardState {InDeck, InHand, InDiscard, InPlay, InQueue}; 

[System.Serializable]
public class Card : MonoBehaviour
{

    public CardState curState;
    private Board board = Board.me;

    private SpriteRenderer[] cardParts;
    [SerializeField]
    private Sprite[] cardSprites;

    private Sequence tweenSequence;
    private Transform tr;
    private Transform prevParent;
    public bool isSettled = true;

    public string cardName;
    public int cost;
    public string desc;
    public Sprite cardArt;

    // FMOD variables
    [FMODUnity.EventRef]
    public string drawSoundEvent;
    [FMODUnity.EventRef]
    public string hoverSoundEvent;
    [FMODUnity.EventRef]
    public string discardSoundEvent;
    [FMODUnity.EventRef]
    public string shuffleSoundEvent;
    [FMODUnity.EventRef]
    public string selectSoundEvent;
    [FMODUnity.EventRef]
    public string deselectSoundEvent;

    FMOD.Studio.EventInstance drawSound;
    FMOD.Studio.EventInstance hoverSound;
    FMOD.Studio.EventInstance discardSound;
    FMOD.Studio.EventInstance shuffleSound;
    FMOD.Studio.EventInstance selectSound;
    FMOD.Studio.EventInstance deselectSound;

    private IEnumerator DrawAnim(Transform tr) {
        tr.localScale = Vector3.zero;
        tr.DOMove(tr.parent.position, .3f);
        tr.DOScale(1f * Vector3.one, .3f);
        yield return null;

    }

    private IEnumerator MulliganAnim(Transform tr) {
        tr.DOMove(tr.parent.position, .3f);
        tr.DOScale(.1f * Vector3.one, .3f);
        yield return new WaitForSeconds(.3f);
        foreach(SpriteRenderer sr in cardParts) { sr.enabled = false; }

    }

    // TODO: try having second param, and iterating delay for each subsequent call to get staggering effect
    private IEnumerator ReshuffleAnim(Transform tr) {
        // cardParts[0].sprite = cardSprites[0];
        // cardParts[2].sprite = cardSprites[2];
        cardParts[0].enabled = true;
        cardParts[2].enabled = true;

        tr.DOMove(tr.parent.position, .6f);
        tr.DOScale(.3f * Vector3.one, .3f);
        tr.DOScale(.1f * Vector3.one, .3f);
        yield return new WaitForSeconds(.6f);
        foreach(SpriteRenderer sr in cardParts) sr.enabled = false;
    }

    void Awake(){
        tweenSequence = DOTween.Sequence();
        cardParts = GetComponentsInChildren<SpriteRenderer>();
        tr = this.gameObject.transform;
        curState = CardState.InDeck;

        // FMOD object init
        drawSound = FMODUnity.RuntimeManager.CreateInstance(drawSoundEvent);
        hoverSound = FMODUnity.RuntimeManager.CreateInstance(hoverSoundEvent);
        discardSound = FMODUnity.RuntimeManager.CreateInstance(discardSoundEvent);
        shuffleSound = FMODUnity.RuntimeManager.CreateInstance(shuffleSoundEvent);
        selectSound = FMODUnity.RuntimeManager.CreateInstance(selectSoundEvent);
        deselectSound = FMODUnity.RuntimeManager.CreateInstance(deselectSoundEvent);
    }

    void Update(){
        cardParts[3].enabled = board.lockedHand.Contains(this.gameObject) && board.curPhase == Phase.Mulligan; // render a lock icon if the card is locked
        
        // tween to the correct pile depending on state
        switch(curState) {
            case CardState.InHand:
                if(!isSettled) {
                    StartCoroutine(DrawAnim(tr));
                    isSettled = true;
                    for(int i = 0; i < 3; i++){
                        cardParts[i].enabled = true;
                        cardParts[1].sprite = cardArt;
                        if(cardSprites[i] != null){
                            cardParts[i].sprite = cardSprites[i];
                        }
                    }
                    // FMOD Draw Event
                    drawSound.start();
                }
                break;

            case CardState.InDiscard:
                if(!isSettled) {
                    StartCoroutine(MulliganAnim(tr));
                    isSettled = true;
                    // FMOD Discard Event
                    discardSound.start();
                }
                break;
            
            case CardState.InDeck:
                if(!isSettled) {
                    StartCoroutine(ReshuffleAnim(tr));
                    isSettled = true;
                    // FMOD Shuffle Event
                    shuffleSound.start();
                }
                break;
            
            case CardState.InPlay:
                Vector3 mousePos = Input.mousePosition;
                tr.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
                break;

            case CardState.InQueue:
                if(!isSettled) {
                    StartCoroutine(DrawAnim(tr));
                    isSettled = true;
                    foreach(SpriteRenderer sr in cardParts){
                        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);
                        sr.sortingLayerName = "UI Low"; 
                    } 
                }
                break;

        }

    }
    void Attack(int amount, Target target){

    }

    void Defend(int amount, Target target){

    }

    void OnMouseEnter(){
        if(curState == CardState.InHand) {
            foreach(SpriteRenderer sr in cardParts) {
                sr.sortingLayerName = "UI High";
            }
            tweenSequence.Append(tr.DOScale(1.4f * Vector3.one, .25f).SetId("zoomIn"));
            tweenSequence.Insert(0, tr.DOMoveZ(-1f, .5f).SetId("zoomIn"));
            // FMOD Hover Event
            hoverSound.start();
        }
        
    }

    void OnMouseExit(){
        if(curState == CardState.InHand) {
            foreach(SpriteRenderer sr in cardParts) {
                sr.sortingLayerName = "UI Low";
            }
            DOTween.Pause("zoomIn");
            tweenSequence.Append(tr.DOScale(Vector3.one, .1f));
        }
        
    }

    void OnMouseDown() {
        switch(board.curPhase){
            case Phase.Mulligan:
                // add card to the mulligan list if it isn't already in, and if it isn't locked, and if the mulligan limit isn't reached
                if(curState == CardState.InHand && !board.toMul.Contains(this.gameObject) && 
                board.toMul.Count < board.mulLimit && !board.lockedHand.Contains(this.gameObject)) {
                    board.toMul.Add(this.gameObject);
                    tr.DOMoveY(tr.position.y + .5f, .1f);
                    // FMOD Card Select Event
                    selectSound.start();
                }
                else if(board.toMul.Contains(this.gameObject) && !board.lockedHand.Contains(this.gameObject)) {
                    board.toMul.Remove(this.gameObject);
                    tr.DOMoveY(tr.position.y - .5f, .1f);
                    // FMOD Card Deselect Event
                    deselectSound.start();
                }
                break;
            case Phase.Play:
                if(curState == CardState.InHand) {
                    curState = CardState.InPlay;
                    prevParent = tr.parent;
                    tr.parent = null;
                }
                break;
            default:
                Debug.Log("reached unknown phase on click");
                break;
        }
    }

    void OnMouseUpAsButton() {
        if(curState == CardState.InPlay) {
            Collider2D[] colliders = Physics2D.OverlapPointAll(new Vector2(transform.position.x, transform.position.y));            
            foreach(Collider2D collider in colliders) {
                if(collider.GetComponent<SpriteRenderer>() == null) continue;
                if(collider.GetComponent<SpriteRenderer>().sortingLayerName == "Targets") {
                    Action toInsert = new Action(this, collider.gameObject);
                    toInsert.completeTime = board.playSequence.totalTime + toInsert.card.cost; // TODO: integrate this calculation as a method on Action?
                    board.playSequence.Add(toInsert);
                    curState = CardState.InQueue;
                }
            }
            // reanchor to old hand pos
            tr.parent = prevParent;
            prevParent = null;
            curState = curState == CardState.InQueue ? CardState.InQueue : CardState.InHand;
            isSettled = false; // initiates tween back to hand pos
        }
    }
    
}
