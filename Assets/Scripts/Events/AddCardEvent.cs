using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AddCardEvent : Event {
    
    public GameObject confirmPrefab;
    private GameObject confirmInstance;
    private DeckDisplay deckDisplay;
    public List<GameObject> toAdd = new List<GameObject>();

    protected override void Start() {
        deckDisplay = GameObject.Find("_DeckRenderer").GetComponent<DeckDisplay>();
        base.Start();
    }
    
    protected void Update() {
        if(toAdd.Count == 2 && confirmInstance == null) {
            InstantiateConfirmBtn();
        } else if(toAdd.Count != 2 && confirmInstance != null) {
            Destroy(confirmInstance);
        }
    }

    protected override void ResolveEvent() {
        board.Reshuffle();
        foreach(Transform container in board.eventContainers) {
            container.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "UI High";
            foreach(TextMeshPro text in container.GetComponentsInChildren<TextMeshPro>()) {
                text.sortingLayerID = SortingLayer.NameToID("UI High");
            }
            container.GetComponentInChildren<BoxCollider2D>().enabled = false;
        }
        deckDisplay.DeckToSelectScreen(board.addDeck, CardState.InSelectionAdd);
    }

    public override void ResolveConfirm() {
        CallBaseResolve();
        GameObject.Find("_DeckRenderer").GetComponent<DeckDisplay>().DeckOffScreen();

        for(int i = this.toAdd.Count - 1; i >= 0; i--) {
            Board.instance.deck.Add(this.toAdd[i]);
            Card cardScript = this.toAdd[i].GetComponent<Card>();
            cardScript.textParts[0].text = cardScript.cardName;
            cardScript.textParts[1].text = cardScript.desc;
            cardScript.textParts[2].text = cardScript.cost.ToString();
            for (int j = 0; j < 3; j++ ) {
                cardScript.textParts[j].sortingOrder = -1;
                cardScript.textParts[j].sortingLayerID = SortingLayer.NameToID("UI High");
            }
            cardScript.isSettled = false;
            cardScript.curState = CardState.InDeck;
            cardScript.transform.parent = Board.instance.cardAnchors["Deck Anchor"];
            Board.instance.addDeck.Remove(this.toAdd[i]);
            this.toAdd.RemoveAt(i);
        }
        Board.instance.FisherYatesShuffle(Board.instance.deck);
        Destroy(confirmInstance);
    }

    public void InstantiateConfirmBtn() {
        // at y -4.5
        confirmInstance = Instantiate(confirmPrefab, new Vector3(0, -4.5f, -2f), Quaternion.identity, this.transform);
        confirmInstance.GetComponent<EventConfirmButton>().eventInstance = this;
    }

    public void CallBaseResolve() {
        base.ResolveEvent();
    }
}
