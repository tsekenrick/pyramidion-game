using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum CardState {InDeck, InHand, InDiscard}; 

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
    public bool isSettled = true;

    public string cardName;
    public int cost;
    public string desc;
    public Sprite cardArt;
    
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
    }

    void Update(){
        switch(curState) {
            case CardState.InHand:
                if(!isSettled) {
                    StartCoroutine(DrawAnim(tr));
                    isSettled = true;
                    for(int i = 0; i < cardParts.Length; i++){
                        cardParts[i].enabled = true;
                        cardParts[1].sprite = cardArt;
                        if(cardSprites[i] != null){
                            cardParts[i].sprite = cardSprites[i];
                        }
                    }
                }
                break;


            case CardState.InDiscard:
                if(!isSettled) {
                    StartCoroutine(MulliganAnim(tr));
                    isSettled = true;
                }
                break;
            
            case CardState.InDeck:
                if(!isSettled) {
                    StartCoroutine(ReshuffleAnim(tr));
                    isSettled = true;
                }
                break;
        }

    }
    void attack(int amount, Target target){

    }

    void defend(int amount, Target target){

    }

    void OnMouseEnter(){
        if(curState == CardState.InHand) {
            tweenSequence.Append(tr.DOScale(1.45f * Vector3.one, .4f).SetId("zoomIn"));
            tweenSequence.Insert(0, tr.DOMoveZ(-1f, .5f).SetId("zoomIn"));
        }
        
    }

    void OnMouseExit(){
        if(curState == CardState.InHand) {
            DOTween.Pause("zoomIn");
            tweenSequence.Append(tr.DOScale(Vector3.one, .1f));
        }
        
    }

    void OnMouseDown(){
        switch(board.curPhase){
            case Phase.Mulligan:
                if(curState == CardState.InHand) {
                    board.ToDiscard(this);
                    board.DrawCard();  
                }        
                break;
            case Phase.Play:
                break;
            default:
                Debug.Log("reached unknown phase on click");
                break;
        }
    }

    
    
}
