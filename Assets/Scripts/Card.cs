﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using DG.Tweening;
using TMPro;

public enum CardState {InDeck, InHand, InDiscard, InPlay, InQueue}; 

[System.Serializable]
public class Card : MonoBehaviour
{

    public CardState curState;
    private Board board = Board.me;
    private SoundManager sm = SoundManager.me;

    public SpriteRenderer[] cardParts;
    public TextMeshPro[] textParts;
    [SerializeField]
    private Sprite[] cardSprites = new Sprite[3];

    // anim related fields
    private Sequence tweenSequence;
    private Transform tr;
    private Transform prevParent;
    public bool isSettled = true;

    // fields read from json
    public string cardName;
    public int cost;
    public string desc;
    public Sprite cardArt;
    public string[] cardProps;

    public GameObject target; // null before card is "played"
    public PlayerAction action; // also null before card is played

    private IEnumerator DrawAnim(Transform tr) {
        tr.localScale = Vector3.zero;
        tr.DOMove(tr.parent.position, .3f);
        tr.DOScale(1f * Vector3.one, .3f);
        yield return null;

    }

    private void PlayAnim(Transform tr) {
        GetComponent<TrailRenderer>().enabled = false;
        tr.localScale = Vector3.zero;
        tr.position = tr.parent.position;
        tr.localScale = Vector3.one;
        GetComponent<TrailRenderer>().enabled = true;
    }

    private IEnumerator MulliganAnim(Transform tr) {
        tr.DOMove(tr.parent.position, .3f);
        tr.DOScale(.1f * Vector3.one, .3f);
        yield return new WaitForSeconds(.3f);
        foreach(SpriteRenderer sr in cardParts) { sr.enabled = false; }
        foreach(TextMeshPro tmp in textParts) tmp.text = "";
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
        foreach(TextMeshPro tmp in textParts) tmp.text = "";
    }

    // this currently does not factor any sort of status modifier pressent on `target`
    public void Attack(int amount, GameObject target) {
        Target t = target.GetComponentInParent<Target>();
        int tmpBlock = t.block;
        t.block = Mathf.Max(t.block - amount, 0);
        t.health -= Mathf.Max(amount - tmpBlock, 0);
    }

    public void Defend(int amount, GameObject target) {
        // Target t = target.GetComponent<Target>();
        Target t = GameObject.Find("Player").GetComponent<Target>(); // hardcoded sins
        t.block += amount;
    }

    public virtual void OnMulligan() {
        return;
    }

    public virtual void OnEnqueue() {
        return;
    }

    public virtual void OnDequeue() {
        return;
    }

    public virtual void resolveAction() {
        MethodInfo mi = this.GetType().GetMethod(this.cardProps[0]);
        switch(cardProps[0]) {
            case "Attack":
                // FMOD Player Attack Sound
                sm = SoundManager.me;
                sm.PlayPlayerAttackSound();
                break;
            case "Defend":
                // FMOD Player Defend Sound
                sm = SoundManager.me;
                sm.PlayPlayerDefendSound();
                break;
        }
        mi.Invoke(this, new object[]{int.Parse(this.cardProps[1]), this.target});
        // do things to resolve action based on data given in card field
    }

    public virtual void Awake(){
        tweenSequence = DOTween.Sequence();
        cardParts = GetComponentsInChildren<SpriteRenderer>();
        textParts = GetComponentsInChildren<TextMeshPro>();
        tr = this.gameObject.transform;
        curState = CardState.InDeck;

        foreach(SpriteRenderer sr in cardParts) sr.enabled = false;
        foreach(TextMeshPro tmp in textParts) tmp.text = "";

    }

    public virtual void Update(){
        if((board.lockedHand.Contains(this.gameObject) || board.lockedHand.Count >= 4) && board.curPhase == Phase.Mulligan) {
            foreach(SpriteRenderer sr in cardParts) {
                sr.color = new Color(.5f, .5f, .5f, 1f);
            }
            cardParts[4].enabled = false; // kill glow
        } else if(curState == CardState.InHand) {
            foreach(SpriteRenderer sr in cardParts) {
                sr.color = Color.white;
            }
            cardParts[4].enabled = true;
        }

        GetComponent<TrailRenderer>().enabled = !(curState == CardState.InQueue);
        cardParts[5].sortingLayerName = "UI High";
        
        if(curState != CardState.InQueue) {
            foreach(SpriteRenderer sr in cardParts){
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            }
        }
         
        // tween to the correct pile depending on state
        switch(curState) {
            case CardState.InHand:
                if(!isSettled) {
                    StartCoroutine(DrawAnim(tr));
                    isSettled = true;
                    textParts[0].text = cardName;
                    textParts[1].text = desc;
                    textParts[2].text = cost.ToString();
                    for(int i = 0; i < 3; i++){
                        cardParts[i].enabled = true;
                        cardParts[1].sprite = cardArt;
                    }

                    foreach(TextMeshPro tmp in textParts) {
                        tmp.sortingLayerID = SortingLayer.NameToID("UI High");
                    }
                    // FMOD Draw Event
                    sm.PlaySound(sm.drawSound);
                }
                break;

            case CardState.InDiscard:
                if(!isSettled) {
                    StartCoroutine(MulliganAnim(tr));
                    isSettled = true;
                    // FMOD Discard Event
                    sm.PlaySound(sm.discardSound);
                }
                break;
            
            case CardState.InDeck:
                if(!isSettled) {
                    StartCoroutine(ReshuffleAnim(tr));
                    isSettled = true;
                    // FMOD Shuffle Event
                    sm.PlaySound(sm.shuffleSound);
                }
                break;
            
            case CardState.InPlay:
                Vector3 mousePos = Input.mousePosition;
                tr.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
                break;

            case CardState.InQueue:
                if(!isSettled) {
                    PlayAnim(tr);
                    isSettled = true;
                    foreach(SpriteRenderer sr in cardParts){
                        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);
                        sr.sortingLayerName = "UI Low"; 
                    } 

                    foreach(TextMeshPro tmp in textParts) {
                        tmp.sortingLayerID = SortingLayer.NameToID("UI Low");
                    }
                    cardParts[4].enabled = false;
                }
                break;

        }
    }
    

    void OnMouseEnter(){
        if(board.overlayActive) return;

        if(curState == CardState.InHand) {
            foreach(SpriteRenderer sr in cardParts) {
                sr.sortingLayerName = "UI High";
                sr.sortingOrder = 6;
            }
            cardParts[5].sortingOrder = 7;
            cardParts[4].sortingOrder = 3; // set glow below the rest
            foreach(TextMeshPro tmp in textParts) tmp.sortingOrder = 10;
            tweenSequence.Append(tr.DOScale(1.4f * Vector3.one, .25f).SetId("zoomIn"));
            //tweenSequence.Insert(0, tr.DOMoveZ(-1f, .5f).SetId("zoomIn"));
            // FMOD Hover Event
            sm.PlaySound(sm.hoverSound);
        }
        
    }

    void OnMouseExit(){
        if(board.overlayActive) return;

        if(curState == CardState.InHand) {
            cardParts[5].sortingOrder = 0;
            foreach(SpriteRenderer sr in cardParts) sr.sortingLayerName = "UI Low";
            foreach(TextMeshPro tmp in textParts) tmp.sortingOrder = 3;
            DOTween.Pause("zoomIn");
            tweenSequence.Append(tr.DOScale(Vector3.one, .1f));
        }
        
    }

    void OnMouseDown() {
        if(board.overlayActive) return;

        switch(board.curPhase){
            case Phase.Mulligan:
                // add card to the mulligan list if it isn't already in, and if it isn't locked, and if the mulligan limit isn't reached
                if(curState == CardState.InHand && !board.toMul.Contains(this.gameObject) && 
                   !board.lockedHand.Contains(this.gameObject)) {
                                    // board.toMul.Count < board.mulLimit &&
                    if(board.toMul.Count == board.mulLimit) {
                        board.toMul[0].GetComponent<Card>().cardParts[5].enabled = false;
                        board.toMul[0] = this.gameObject;
                    } else {
                        board.toMul.Add(this.gameObject);
                    }
                    cardParts[5].enabled = true;
                    cardParts[5].sortingOrder = 15;
                    // FMOD Card Select Event
                    
                    sm.PlaySound(sm.selectSound);
                }
                else if(board.toMul.Contains(this.gameObject) && !board.lockedHand.Contains(this.gameObject)) {
                    board.toMul.Remove(this.gameObject);
                    cardParts[5].enabled = false;
                    // FMOD Card Deselect Event
                    sm.PlaySound(sm.deselectSound);
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
                if(collider.GetComponentInParent<SpriteRenderer>() == null) continue;

                if(collider.GetComponentInParent<SpriteRenderer>().sortingLayerName == "Targets") {
                    PlayerAction toInsert = new PlayerAction(this, collider.gameObject);
                    this.action = toInsert;
                    this.target = collider.gameObject;
                    toInsert.completeTime = board.playSequence.totalTime + toInsert.card.cost; // TODO: integrate this calculation as a method on Action?
                    board.playSequence.Add(toInsert);
                    OnEnqueue();
                    curState = CardState.InQueue;
                    // FMOD Card Play Confirmation Sound
                    sm.PlaySound(sm.confirmCardSound);
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
