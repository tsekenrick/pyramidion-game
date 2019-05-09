using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class DeckDisplay : MonoBehaviour
{
    private Board board;
    public static DeckDisplay instance;  

    public SpriteRenderer screenOverlay;
    public bool isRendering;
    public List<GameObject> curRender;
    private SpriteRenderer[] lastRendered;
    private TextMeshPro lastRenderedCounter;

    private Transform oldParent; // 

    void Start() { 
        isRendering = false;
        board = Board.me;
        instance = this; 
    }

    void Update() {

        // define completeDeck when needed, not on update
        // completeDeck = board.deck;
        // completeDeck.AddRange(board.discard);
        // completeDeck = board.FisherYatesShuffle(completeDeck);

        // reset doesn't quite work with discard pile rn
        if(Input.GetKeyDown(KeyCode.Escape) && isRendering && !(board.curPhase == Phase.Event)) {
            screenOverlay.enabled = false;
            if(lastRendered != null) {
                foreach(SpriteRenderer sr in lastRendered) sr.sortingLayerName = "UI Mid";
                lastRenderedCounter.sortingLayerID = SortingLayer.NameToID("UI Mid");
            }

            if(curRender.Count == 0) {
                curRender = null;
                isRendering = false;
                return;
            }

            foreach(GameObject go in curRender) {
                go.transform.parent = oldParent;
                go.GetComponent<TrailRenderer>().enabled = true;
                go.GetComponent<Card>().isSettled = false;
                for(int i = 0; i < 3; i++){
                    go.GetComponent<Card>().textParts[i].sortingLayerID = SortingLayer.NameToID("UI High");
                    go.GetComponent<Card>().textParts[i].sortingOrder = -1;
                    go.GetComponent<Card>().cardParts[i].sortingLayerName = "UI Low";
                }
            }
            curRender = null;
            isRendering = false;
        }
    }
    
    public void DeckOffScreen(SpriteRenderer[] toLower = null, TextMeshPro toLowerText = null) {
        screenOverlay.enabled = false;
        if(toLower != null) {
            foreach(SpriteRenderer sr in toLower) sr.sortingLayerName = "UI Mid";
            toLowerText.sortingLayerID = SortingLayer.NameToID("UI Mid");
        }

        if(curRender.Count == 0) {
            curRender = null;
            isRendering = false;
            return;
        }

        foreach(GameObject go in curRender) {
            go.transform.parent = oldParent;
            go.GetComponent<TrailRenderer>().enabled = true;
            go.GetComponent<Card>().isSettled = false;
            // for case of displaying during events, always goes back to deck afterwards
            if(go.GetComponent<Card>().curState == CardState.InSelection) go.GetComponent<Card>().curState = CardState.InDeck;
            
            for(int i = 0; i < 3; i++){
                go.GetComponent<Card>().textParts[i].sortingLayerID = SortingLayer.NameToID("UI High");
                go.GetComponent<Card>().textParts[i].sortingOrder = -1;
                go.GetComponent<Card>().cardParts[i].sortingLayerName = "UI Low";
            }
        }
        curRender = null;
        isRendering = false;
    }

    public void DeckToScreen(List<GameObject> toRender, SpriteRenderer[] toRaise = null, TextMeshPro toRaiseText = null) {
        screenOverlay.enabled = true;       
        if(toRaise != null) {
            lastRendered = toRaise;
            lastRenderedCounter = toRaiseText;
            foreach(SpriteRenderer sr in toRaise) sr.sortingLayerName = "Above Darkness";
            toRaiseText.sortingLayerID = SortingLayer.NameToID("Above Darkness");
        }
        curRender = toRender;
        isRendering = true;

        if(toRender.Count == 0) return;

        // 3, -8
        // plus 6 in x per card
        // plus 8 in y per row
        float xMin = -8f;
        float yMin = 3.7f;
        oldParent = toRender[0].transform.parent;
        for(int i = 0; i < toRender.Count; i++) {
            int row = i / 8;
            int col = i % 8;
            GameObject card = toRender[i];
            Card cardScript = card.GetComponent<Card>();
            foreach(SpriteRenderer sr in cardScript.cardParts) {
                if(sr != cardScript.cardParts[4]) sr.color = Color.white;
            }
            card.GetComponent<TrailRenderer>().enabled = false;
            card.transform.parent = this.gameObject.transform;
            
            cardScript.textParts[0].text = cardScript.cardName;
            cardScript.textParts[1].text = cardScript.desc;
            cardScript.textParts[2].text = cardScript.cost.ToString();

            cardScript.cardParts[1].sprite = cardScript.cardArt;
            for(int j = 0; j < 3; j++) {
                cardScript.textParts[j].sortingLayerID = SortingLayer.NameToID("Above Darkness");
                cardScript.textParts[j].sortingOrder = 7;
                cardScript.cardParts[j].enabled = true; 
                cardScript.cardParts[j].sortingLayerName = "Above Darkness";
            }
            card.transform.localScale = Vector3.one * .8f;
            card.transform.position = new Vector3(xMin + (2.275f * col), yMin - (row * 2.5f), 0);
        }
    }

    public void DeckToSelectScreen(List<GameObject> toRender) {
        screenOverlay.enabled = true;
        curRender = toRender;
        isRendering = true;

        float xMin = -8f;
        float yMin = 3.7f;
        oldParent = toRender[0].transform.parent;
        for(int i = 0; i < toRender.Count; i++) {
            int row = i / 8;
            int col = i % 8;
            GameObject card = toRender[i];
            Card cardScript = card.GetComponent<Card>();
            cardScript.curState = CardState.InSelection;
            foreach(SpriteRenderer sr in cardScript.cardParts) {
                if(sr != cardScript.cardParts[4]) sr.color = Color.white;
            }
            card.GetComponent<TrailRenderer>().enabled = false;
            card.transform.parent = this.gameObject.transform;
            
            cardScript.textParts[0].text = cardScript.cardName;
            cardScript.textParts[1].text = cardScript.desc;
            cardScript.textParts[2].text = cardScript.cost.ToString();

            cardScript.cardParts[1].sprite = cardScript.cardArt;
            for(int j = 0; j < 3; j++) {
                cardScript.textParts[j].sortingLayerID = SortingLayer.NameToID("Above Darkness");
                cardScript.textParts[j].sortingOrder = 7;
                cardScript.cardParts[j].enabled = true; 
                cardScript.cardParts[j].sortingLayerName = "Above Darkness";
            }
            card.transform.localScale = Vector3.one * .8f;
            card.transform.position = new Vector3(xMin + (2.275f * col), yMin - (row * 2.5f), 0);
        }
    }
}
