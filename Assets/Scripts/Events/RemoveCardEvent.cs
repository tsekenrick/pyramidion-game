using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RemoveCardEvent : Event {
    
    public GameObject confirmPrefab;
    private GameObject confirmInstance;
    private DeckDisplay deckDisplay;
    public List<GameObject> toRemove = new List<GameObject>();

    protected override void Start() {
        deckDisplay = GameObject.Find("_DeckRenderer").GetComponent<DeckDisplay>();
        base.Start();
    }

    protected void Update() {
        if(toRemove.Count == 2 && confirmInstance == null) {
            InstantiateConfirmBtn();
        } else if(toRemove.Count != 2 && confirmInstance != null) {
            Destroy(confirmInstance);
        }
    }

    public override void ResolveConfirm() {
        CallBaseResolve();
        GameObject.Find("_DeckRenderer").GetComponent<DeckDisplay>().DeckOffScreen();
        for(int i = this.toRemove.Count - 1; i >= 0; i--) {
            board.deck.Remove(this.toRemove[i]);
            Destroy(this.toRemove[i]);
        }
        Destroy(confirmInstance);
    }

    public void InstantiateConfirmBtn() {
        confirmInstance = Instantiate(confirmPrefab, new Vector3(0, -4.5f, -2f), Quaternion.identity, this.transform);
        confirmInstance.GetComponent<EventConfirmButton>().eventInstance = this;
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
        deckDisplay.DeckToSelectScreen(board.deck, CardState.InSelectionRemove);
    }

    public void CallBaseResolve() {
        base.ResolveEvent();
    }
    
}
