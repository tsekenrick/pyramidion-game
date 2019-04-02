using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    private Board board;
    public Sprite[] mulliganButtons;
    public Sprite[] executeButtons;
    private SpriteRenderer sr;
    private SpriteRenderer glow;
    private CircleCollider2D collider;
    public bool buttonPressed;
    private bool renderPressed;

    void Start() {
        buttonPressed = false;
        board = Board.me;
        collider = this.GetComponent<CircleCollider2D>();
        sr = this.GetComponent<SpriteRenderer>();
        glow = GameObject.Find("ActionBtnGlow").GetComponent<SpriteRenderer>();
    }

    void Update() {
        glow.enabled = (board.curPhase != Phase.Resolution);

        switch(board.curPhase) {
            case Phase.Mulligan:
                sr.sprite = renderPressed ? mulliganButtons[1] : mulliganButtons[0];
                break;

            case Phase.Play:
                sr.sprite = renderPressed ? executeButtons[1] : executeButtons[0];
                break;

            case Phase.Resolution:
                sr.sprite = executeButtons[0];
                break;

        }
    }

    void OnMouseDown() {
        renderPressed = true;
    }


    void OnMouseUpAsButton() {
        switch(board.curPhase){
            case Phase.Mulligan:
                Debug.Log("hit");
                buttonPressed = true;
                renderPressed = false;
                break;
            
            case Phase.Play:
                buttonPressed = true;
                renderPressed = false;
                break;
        }
    }
}
