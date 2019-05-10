using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour {
    
    [SerializeField]
    protected Sprite[] buttonStates;

    void OnMouseEnter() {
        GetComponent<SpriteRenderer>().sprite = buttonStates[1];
    }

    void OnMouseExit() {
        GetComponent<SpriteRenderer>().sprite = buttonStates[0];
    }

    void OnMouseDown() {
        GetComponent<SpriteRenderer>().sprite = buttonStates[2];
    }

    protected virtual void OnMouseUpAsButton() {
        GetComponent<SpriteRenderer>().sprite = buttonStates[0];
    }

}
