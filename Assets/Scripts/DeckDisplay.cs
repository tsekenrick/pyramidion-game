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
    public bool canToggle;

    void Start() { 
        canToggle = true;
        isRendering = false;
        board = Board.instance;
        instance = this; 
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) && canToggle && isRendering && !(board.curPhase == Phase.Event)) {
            DeckOffScreen(lastRendered, lastRenderedCounter);
        }
    }
    
    private IEnumerator SpamDisabler() {
        canToggle = false;
        yield return new WaitForSeconds(.2f);
        canToggle = true;
    }

    public void DeckOffScreen(SpriteRenderer[] toLower = null, TextMeshPro toLowerText = null) {
        if(!canToggle) return;
        StartCoroutine(SpamDisabler());

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
            Card cardScript = go.GetComponent<Card>();
            go.transform.parent = oldParent;
            if(cardScript.curState != CardState.InSelectionAdd) go.GetComponent<TrailRenderer>().enabled = true;
            StartCoroutine(cardScript.MulliganAnim(cardScript.transform));
            // for case of displaying during events, always goes back to deck afterwards
            if(cardScript.curState == CardState.InSelectionRemove || cardScript.curState == CardState.InSelectionAdd) {
                foreach(SpriteRenderer sr in cardScript.cardParts) {
                    if(sr != cardScript.cardParts[5] && sr != cardScript.cardParts[3]) {
                        sr.enabled = false;
                    }
                }
                cardScript.curState = CardState.InDeck;
            }
            for(int i = 0; i < 3; i++){
                cardScript.textParts[i].sortingLayerID = SortingLayer.NameToID("UI High");
                cardScript.textParts[i].sortingOrder = -1;
                cardScript.textParts[i].enabled = false;
                cardScript.cardParts[i].sortingLayerName = "UI Low";
            }
            cardScript.cardParts[4].sortingLayerName = "UI Low";


        }
        curRender = null;
        isRendering = false;
    }

    public void DeckToScreen(List<GameObject> toRender, SpriteRenderer[] toRaise = null, TextMeshPro toRaiseText = null) {
        if(!canToggle) return;
        StartCoroutine(SpamDisabler());
        
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
                if(sr != cardScript.cardParts[5] && sr != cardScript.cardParts[3]) {
                    sr.enabled = true;
                }
            }
            card.GetComponent<TrailRenderer>().enabled = false;
            card.GetComponent<BoxCollider2D>().enabled = true;
            card.transform.parent = this.gameObject.transform;
            
            cardScript.textParts[0].text = cardScript.cardName;
            cardScript.textParts[1].text = cardScript.desc;
            cardScript.textParts[2].text = cardScript.cost.ToString();

            cardScript.cardParts[1].sprite = cardScript.cardArt;
            for(int j = 0; j < 3; j++) {
                cardScript.textParts[j].sortingLayerID = SortingLayer.NameToID("Above Darkness");
                cardScript.textParts[j].sortingOrder = 7;
                cardScript.textParts[j].enabled = true;
                cardScript.cardParts[j].enabled = true; 
                cardScript.cardParts[j].sortingLayerName = "Above Darkness";
            }
            card.transform.localScale = Vector3.one * .8f;
            card.transform.position = new Vector3(xMin + (2.275f * col), yMin - (row * 2.5f), 0);
        }
    }

    public void DeckToSelectScreen(List<GameObject> toRender, CardState state) {
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
            cardScript.curState = state;
            foreach(SpriteRenderer sr in cardScript.cardParts) {
                if(sr != cardScript.cardParts[4]) sr.color = Color.white;
                if(sr != cardScript.cardParts[5] && sr != cardScript.cardParts[3]) {
                    sr.enabled = true;
                }
            }
            card.GetComponent<TrailRenderer>().enabled = false;
            card.GetComponent<BoxCollider2D>().enabled = true;
            card.transform.parent = this.gameObject.transform;
            
            cardScript.textParts[0].text = cardScript.cardName;
            cardScript.textParts[1].text = cardScript.desc;
            cardScript.textParts[2].text = cardScript.cost.ToString();

            cardScript.cardParts[1].sprite = cardScript.cardArt;
            for(int j = 0; j < 3; j++) {
                cardScript.textParts[j].sortingLayerID = SortingLayer.NameToID("Above Darkness");
                cardScript.textParts[j].sortingOrder = 7;
                cardScript.textParts[j].enabled = true;
                cardScript.cardParts[j].enabled = true; 
                cardScript.cardParts[j].sortingLayerName = "Above Darkness";
            }
            card.transform.localScale = Vector3.one * .8f;
            card.transform.position = new Vector3(xMin + (2.275f * col), yMin - (row * 2.5f), 0);
        }
    }
}
