using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RemoveCardEvent : Event {
    
    private DeckDisplay deckDisplay;
    public List<GameObject> toRemove = new List<GameObject>();

    protected override void Start() {
        deckDisplay = GameObject.Find("_DeckRenderer").GetComponent<DeckDisplay>();
        base.Start();
    }

    void Update() {

    }

    protected override void resolveEvent() {
        board.Reshuffle();
        foreach(Transform container in board.eventContainers) {
            container.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "UI High";
            foreach(TextMeshPro text in container.GetComponentsInChildren<TextMeshPro>()) {
                text.sortingLayerID = SortingLayer.NameToID("UI High");
            }
            container.GetComponentInChildren<BoxCollider2D>().enabled = false;
        }
        deckDisplay.DeckToSelectScreen(board.deck);
    }

    public void callBaseResolve() {
        base.resolveEvent();
    }
    
}
