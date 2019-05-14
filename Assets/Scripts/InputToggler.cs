using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputToggler : MonoBehaviour
{
    private Board board;
    void Start() { 
        board = Board.instance;
    }

    void Update() {
        board.overlayActive = this.GetComponent<SpriteRenderer>().enabled;
    }
}
