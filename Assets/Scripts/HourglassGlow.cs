using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HourglassGlow : MonoBehaviour
{
    private Board board;
    public bool isActive;

    void Start() {
        isActive = false;
        board = Board.instance;
    }

    void Update() {
        if(isActive) {
            this.GetComponent<SpriteRenderer>().enabled = true;
            this.GetComponent<SpriteRenderer>().color = board.borrowedTime < 0 ? new Color(.55f, .85f, .43f, .8f) : new Color(1, 0, 0, .7f);
        } else {
            this.GetComponent<SpriteRenderer>().enabled = false;
        }
        
    }
}
