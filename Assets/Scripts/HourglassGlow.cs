using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HourglassGlow : MonoBehaviour
{
    private Board board;
    public bool isActive;

    void Start() {
        isActive = false;
        board = Board.me;
    }

    void Update() {
        if(isActive) {
            this.GetComponent<SpriteRenderer>().enabled = true;
            this.GetComponent<SpriteRenderer>().color = board.borrowedTime < 0 ? Color.green : Color.red;
        } else {
            this.GetComponent<SpriteRenderer>().enabled = false;
        }
        
    }
}
