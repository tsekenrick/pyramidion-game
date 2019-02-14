using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Card : MonoBehaviour
{
    enum CardState {inDeck, inHand, inDiscard}; 
    public GameObject card;
    private Board board = Board.me;

    void attack(int amount, Target target){

    }

    void defend(int amount, Target target){

    }

    void Start(){
        card = GetComponent<GameObject>();
        CardState cardState = CardState.inDeck;
        card.transform.DOMove(new Vector3(3,3,3),1);
    }
    
}
