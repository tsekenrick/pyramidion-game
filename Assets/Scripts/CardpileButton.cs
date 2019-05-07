using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardpileButton : MonoBehaviour
{
    private Board board;
    private List<GameObject> thisPile;
    
    void Start() {
        board = Board.me;

        switch(this.gameObject.name) {
            case "_DiscardAnchor":
                thisPile = board.discard;
                break;

            case "_DeckAnchor":
                thisPile = board.FisherYatesShuffle(board.deck);
                break;
            
            default:
                Debug.LogError("attempting to click on unidentified cardpile");
                break;
        }
    }

    void Update() {
        this.GetComponentInChildren<TextMeshPro>().text = thisPile.Count.ToString();
        switch(this.gameObject.name) {
            case "_DiscardAnchor":
                thisPile = board.discard;
                break;

            case "_DeckAnchor":
                thisPile = board.FisherYatesShuffle(board.deck);
                break;
            
            default:
                Debug.LogError("attempting to click on unidentified cardpile");
                break;
        }
    }

    void OnMouseEnter() {
        SpriteRenderer sr = this.transform.Find("DeckGlow").GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
    }

    void OnMouseExit() {
        SpriteRenderer sr = this.transform.Find("DeckGlow").GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .55f);
    }

    void OnMouseUpAsButton() {
        // if(board.overlayActive) return;
        DeckDisplay overlay = GameObject.Find("_DeckRenderer").GetComponent<DeckDisplay>();
        if(overlay.isRendering) {
            overlay.DeckOffScreen();
        } else {
            overlay.DeckToScreen(thisPile);
        }
    }
}
