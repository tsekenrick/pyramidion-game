using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum Phase {Mulligan, Play, Resolution};
public class Board : MonoBehaviour
{
    public string deckFileName = "deck.json";

    public Phase curPhase;
    
    // "entity" fields
    public static Board me;
    public GameObject player;
    public GameObject[] enemies;

    // card manipulating fields
    public GameObject cardPrefab;
    private List<GameObject> pool = new List<GameObject>();
    public Dictionary<string, Transform> cardAnchors = new Dictionary<string, Transform>();

    public Queue<Card> deck;
    public List<Card> discard = new List<Card>();
    public List<GameObject> hand = new List<GameObject>();
    public int turn;

    [System.Serializable]
    public class DeckList{
        public List<CardData> deckList;
        public DeckList(){
            deckList = new List<CardData>();
        }
    }
    
    [System.Serializable]
    public class CardData{
        public string cardName;
        public int cost;
        public string desc;
    }

    DeckList loadDeckData(){
        string path = Path.Combine(Application.streamingAssetsPath, deckFileName);
        
        if(File.Exists(path)){
            string data = File.ReadAllText(path);
            DeckList parsed = JsonUtility.FromJson<DeckList>(data);
            return parsed;
        } else {
            Debug.Log("ERROR: Failed to read deck data from json");
            return new DeckList();
        }
    }

    void Awake(){
        me=this;
    }

    void Start(){

        List<CardData> deckList = loadDeckData().deckList;
        foreach(CardData card in deckList){
            Debug.Log(card.cardName);
        }
        
        curPhase = Phase.Mulligan;

        // get anchor positions
        cardAnchors.Add("Deck Anchor", GameObject.Find("_DeckAnchor").transform);
        for(int i = 0; i < 5; i++){
            cardAnchors.Add($"Hand {i}", GameObject.Find($"Hand{i}").transform);
        }
        cardAnchors.Add("Discard Anchor", GameObject.Find("_DiscardAnchor").transform);

        // TODO: populate deck by parsing the CardData list - as we instantiate card objects set their respective name, desc and cost,
        // then enqueue the object into the ACTUAL deck, then call a shuffle function at the end of the loop
        for(int i = 0; i < 20; i++){ // change to deckList.Count later
            GameObject curCard = Instantiate(cardPrefab, cardAnchors["Deck Anchor"].position, Quaternion.identity);
            pool.Add(curCard);
            curCard.SetActive(false);
        }
        // maybe get rid of the object pool implementation? or just have some on standby in a separate loop
        // TODO: change below loop to just dequeue cards until your hand is full

        // find inactive gameobjects in pool, activate then and set state to inhand
        for(int i = 0; i < 20 && hand.Count < 5; i++){
            GameObject curCard = pool[i];
            if(!curCard.activeSelf){
                curCard.SetActive(true);
                curCard.GetComponent<Card>().curState = CardState.InHand;
                hand.Add(curCard);
            }
        }
    }
    void Update(){
        switch(curPhase){
            case Phase.Mulligan:
                break;
            case Phase.Play:
                break;
            case Phase.Resolution:
                break;
        }
            
    }
}
