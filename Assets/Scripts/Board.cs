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
    private Dictionary<string, Sprite> cardArtDict = new Dictionary<string, Sprite>();
    public Dictionary<string, Transform> cardAnchors = new Dictionary<string, Transform>();

    public Queue<GameObject> deck = new Queue<GameObject>();
    public List<GameObject> discard = new List<GameObject>();
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
        public string artPath;
    }

    private void GetAnchors() {
        cardAnchors.Add("Deck Anchor", GameObject.Find("_DeckAnchor").transform);
        for(int i = 0; i < 5; i++){
            cardAnchors.Add($"Hand {i}", GameObject.Find($"Hand{i}").transform);
        }
        cardAnchors.Add("Discard Anchor", GameObject.Find("_DiscardAnchor").transform);
    }

    private DeckList loadDeckData(){
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

    private void LoadCardArt(string path, string cardName) {
        WWW www = new WWW(path);
        cardArtDict[cardName] = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
    }

    private List<GameObject> FisherYatesShuffle(List<GameObject> list) {
        for (int i = 0; i < list.Count; i++) {
            GameObject temp = list[i];
            int randIdx = Random.Range(i, list.Count);
            list[i] = list[randIdx];
            list[randIdx] = temp;
        }

        return list;
    }

    void Awake(){
        me=this;
    }

    void Start(){

        List<CardData> deckList = loadDeckData().deckList;
        curPhase = Phase.Mulligan;
        GetAnchors(); // get anchor positions
        
        foreach(CardData card in deckList){
            // load card art into dictionary
            string path = "file://" + Path.Combine(Application.streamingAssetsPath, card.artPath);
            Debug.Log(path);
            LoadCardArt(path, card.cardName);

            // create card gameobject and populate its properties
            GameObject curCard = Instantiate(cardPrefab, cardAnchors["Deck Anchor"].position, Quaternion.identity);
            Card cardScript = curCard.GetComponent<Card>();
            cardScript.cardName = card.cardName;
            cardScript.cost = card.cost;
            cardScript.desc = card.desc;
            cardScript.cardArt = cardArtDict[cardScript.cardName]; 
            pool.Add(curCard);
        }
        pool = FisherYatesShuffle(pool);
        
        // now that all the preloading is done, actually put cards into the deck
        foreach(GameObject card in pool) {
            deck.Enqueue(card);
        }

        // dequeue from deck to draw
        // TODO: add shuffling logic for if deck is empty
        for(int i = 0; hand.Count < 5; i++){
            GameObject curCard = deck.Dequeue();
            curCard.GetComponent<Card>().curState = CardState.InHand;
            hand.Add(curCard);
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
