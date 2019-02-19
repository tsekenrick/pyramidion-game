using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum CardState {InDeck, InHand, InDiscard}; 
public class Card : MonoBehaviour
{
    
    public CardState thisState;
    Transform tr;
    private Board board = Board.me;
    SpriteRenderer[] cardParts;
    public Sprite[] cardSprites;

    void Awake(){
        cardParts = GetComponentsInChildren<SpriteRenderer>();
        tr = this.gameObject.transform;
        thisState = CardState.InDeck;
    }

    void Start(){
        // card.transform.DOMove(new Vector3(3,3,3),1);
    }

    void Update(){
        // draw card iff in hand
        if(thisState == CardState.InHand){
            // turn on each sprite renderer in children then draw the assigned sprite
            for(int i = 0; i < cardParts.Length; i++){
                cardParts[i].enabled = true;
                if(cardSprites[i] != null){
                    cardParts[i].sprite = cardSprites[i];
                }
            }

            // move card into an empty anchor
            for(int i = 0; i < 5; i++){
                Transform anchor = GameObject.Find($"Hand{i}").transform;
                if(anchor.childCount == 0){
                    Debug.Log($"anchor position is {anchor.position}");
                    tr.parent = anchor;
                    tr.transform.DOMove(anchor.position, 1);
                }
            }
        } else {
            foreach(SpriteRenderer sr in cardParts){
                sr.enabled = false;
            }
        }
    }
    void attack(int amount, Target target){

    }

    void defend(int amount, Target target){

    }

    
    
}
