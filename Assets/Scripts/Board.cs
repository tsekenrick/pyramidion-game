using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum Phase { Mulligan, Play, Resolution };
public class Board : MonoBehaviour {
    private string deckFileName = "deck.json";
    public Phase curPhase;
    
    // "entity" fields
    public static Board me;
    public GameObject player;
    public GameObject[] enemies;

    // CARD MANIPULATING FIELDS //
    public GameObject cardPrefab;
    private List<GameObject> pool = new List<GameObject>();
    private Dictionary<string, Sprite> cardArtDict = new Dictionary<string, Sprite>();
    public Dictionary<string, Transform> cardAnchors = new Dictionary<string, Transform>();

    public Queue<GameObject> deck = new Queue<GameObject>();
    public List<GameObject> discard = new List<GameObject>();
    public List<GameObject> hand = new List<GameObject>();
    public int deckCount;

    // MULLIGAN PHASE VARIABLES //
    public int turn;
    public int mulLimit;
    public List<GameObject> toMul = new List<GameObject>(); // is a subset of `hand`
    public List<GameObject> lockedHand = new List<GameObject>(); // also subset of `hand`, union with `toMul` is equal to `hand`

    // PLAY PHASE VARIABLES //
    public PlaySequence<Action> playSequence = new PlaySequence<Action>();

    // FMOD variables
    [FMODUnity.EventRef]
    public string lockSoundEvent;

    FMOD.Studio.EventInstance lockSound;


    [System.Serializable]
    public class DeckList {
        public List<CardData> deckList;
        public DeckList(){
            deckList = new List<CardData>();
        }
    }
    
    [System.Serializable]
    public class CardData {
        public string cardName;
        public int cost;
        public string desc;
        public string artPath;
    }

    // Action describes an action that can be enqueued during the play phase.
    // This includes the card to be played for the action, its target(s), and
    // the time cost of the action.
    public class Action {
        public Card card;
        public GameObject target;
        public int cost;

        public Action(Card card, GameObject target, int cost) {
            this.card = card;
            this.target = target;
            this.cost = cost;
        }
        
    }

    // Extension of List, used to model the sequence of actions created
    // during the play phase, and executed during the resolution phase.
    public class PlaySequence<T> : List<T> {
        public List<Action> sequence;
        public int totalTime;

        public PlaySequence() {
            sequence = new List<Action>();
            totalTime = 0;
        }

        public new void Add(T item) {
            base.Add(item);
            if(item.GetType() == typeof(Board.Action)) {
                Board.Action action = item as Action;
                totalTime += action.cost;
            }
        }

        public new void Remove(T item) {
            base.Remove(item);
            if(item.GetType() == typeof(Board.Action)) {
                Board.Action action = item as Action;
                totalTime -= action.cost;
            }
        }
    }
    
    public void Mulligan(Card card) {
        if(hand.Contains(card.gameObject)) {
            card.isSettled = false;
            card.curState = CardState.InDiscard;
            discard.Add(card.gameObject);
            hand.Remove(card.gameObject);
            card.transform.parent = cardAnchors["Discard Anchor"];
        } else {
            Debug.LogError("attempted to discard card that was not in hand");
        }
    }
    
    public void DrawCard() {
        if(deck.Count == 0) Reshuffle();

        GameObject curCard = deck.Dequeue();
        curCard.GetComponent<Card>().curState = CardState.InHand;
        hand.Add(curCard);
        // find empty hand anchor
        for(int i = 0; i < 5; i++) {
            Transform anchor = GameObject.Find($"Hand{i}").transform;
                if(anchor.childCount == 0){
                    curCard.transform.parent = anchor;
                    curCard.GetComponent<Card>().isSettled = false;
                }
        }
    }

    public void Reshuffle() {
        discard = FisherYatesShuffle(discard);
        foreach(GameObject card in discard) {
            deck.Enqueue(card);
            Card curCard = card.GetComponent<Card>();
            curCard.isSettled = false;
            curCard.curState = CardState.InDeck;
            curCard.transform.parent = cardAnchors["Deck Anchor"];
        }
        discard.Clear();
    }

    private void GetAnchors() {
        cardAnchors.Add("Deck Anchor", GameObject.Find("_DeckAnchor").transform);
        for(int i = 0; i < 5; i++){
            cardAnchors.Add($"Hand {i}", GameObject.Find($"Hand{i}").transform);
        }
        cardAnchors.Add("Discard Anchor", GameObject.Find("_DiscardAnchor").transform);
    }

    private DeckList LoadDeckData(){
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

        List<CardData> deckList = LoadDeckData().deckList;
        GetAnchors(); // get anchor positions
        curPhase = Phase.Mulligan;
        mulLimit = 4;
        turn = 0;
        
        foreach(CardData card in deckList){
            // load card art into dictionary
            string path = "file://" + Path.Combine(Application.streamingAssetsPath, card.artPath);
            LoadCardArt(path, card.cardName);

            // create card gameobject and populate its properties
            GameObject curCard = Instantiate(cardPrefab, cardAnchors["Deck Anchor"].position, Quaternion.identity);
            curCard.transform.parent = cardAnchors["Deck Anchor"];
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

        while(hand.Count < 5){
            DrawCard();
        }

        // FMOD object init
        lockSound = FMODUnity.RuntimeManager.CreateInstance(lockSoundEvent);
    }

    void Update(){
        deckCount = deck.Count; // exposes variable for debug
        switch(curPhase){
            case Phase.Mulligan:
                if(mulLimit == 0) {
                    Debug.Log("now in play phase");
                    curPhase = Phase.Play;
                    turn = 0;
                } else if(Input.GetKeyDown(KeyCode.E)) {
                    turn++;
                    foreach(GameObject card in hand) {
                        if(!toMul.Contains(card) && !lockedHand.Contains(card)) {
                            lockedHand.Add(card);
                        }
                    }

                    foreach(GameObject card in toMul) {
                        Mulligan(card.GetComponent<Card>()); 
                        DrawCard();

                        // FMOD Play Lock Sound
                        lockSound.start();
                    }
                    mulLimit = Mathf.Min(4 - turn, 4 - lockedHand.Count);
                    toMul.Clear();

                }
                break;
            case Phase.Play:
                break;
            case Phase.Resolution:
                break;
        }
            
    }
}
