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
            this.GetComponent<SpriteRenderer>().color = board.borrowedTime < 0 ? new Color(.06f, .59f, .15f, .7f) : new Color(1, 0, 0, .7f);
        } else {
            this.GetComponent<SpriteRenderer>().enabled = false;
        }
        
    }
}
