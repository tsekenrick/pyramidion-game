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
        if(board.overlayActive) return;
        
        board.playSequence.DequeuePlayerAction(thisAction);
    }
}
