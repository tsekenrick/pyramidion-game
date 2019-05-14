using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionInstance : MonoBehaviour {

    public PlayerAction thisAction; 
    private Board board;

    void Start() {
        board = Board.instance;
    }

    void OnMouseEnter() {
        if(thisAction.card.cardProps[0] == "Attack") {
            thisAction.card.target.transform.parent.Find("TargetingFrame").GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    void OnMouseExit() {
        if(thisAction.card.cardProps[0] == "Attack") {
            thisAction.card.target.transform.parent.Find("TargetingFrame").GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    void OnMouseDown() {
        if(thisAction.card.cardProps[0] == "Attack") {
            thisAction.card.target.transform.parent.Find("TargetingFrame").GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    void OnMouseUpAsButton() {
        if(board.overlayActive) return;

        thisAction.card.OnDequeue();
        board.playSequence.DequeuePlayerAction(thisAction);
    }
}
