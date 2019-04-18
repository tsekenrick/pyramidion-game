using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionInstance : MonoBehaviour {

    public PlayerAction thisAction; 
    private Board board;

    void Start() {
        board = Board.me;
    }


    void OnMouseUpAsButton() {
        Debug.Log("hit");
        board.playSequence.DequeuePlayerAction(thisAction);
        // TODO: make sure it goes back to hand lol
    }
}
