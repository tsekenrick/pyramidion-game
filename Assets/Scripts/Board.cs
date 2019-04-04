using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum Phase { Mulligan, Play, Resolution };
public class Board : MonoBehaviour {
    private string deckFileName = "deck.json";
    public Phase curPhase;
    public int borrowedTime; // offset time carryover if overplay/underplay
    public int round; // the number of mul-play-res cycles

    // "entity" fields
    public static Board me;
    public GameObject player;
    public GameObject[] enemies;
    public bool actionButtonPressed;

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
    public int turn; // number of mulligan "sets" completed
    public int mulLimit;
    public List<GameObject> toMul = new List<GameObject>(); // is a subset of `hand`
    public List<GameObject> lockedHand = new List<GameObject>(); // also subset of `hand`, union with `toMul` is equal to `hand`

    // PLAY PHASE VARIABLES //
    public PlaySequence<Action> playSequence = new PlaySequence<Action>();

    // FMOD variables
    [FMODUnity.EventRef]
    public string lockSoundEvent;
    [FMODUnity.EventRef]
    public string toPlayPhaseSoundEvent;
    [FMODUnity.EventRef]
    public string toResolutionPhaseSoundEvent;
    [FMODUnity.EventRef]
    public string toMulliganPhaseSoundEvent;

    FMOD.Studio.EventInstance lockSound;
    FMOD.Studio.EventInstance toPlayPhaseSound;
    FMOD.Studio.EventInstance toResolutionPhaseSound;
    FMOD.Studio.EventInstance toMulliganPhaseSound;

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
        public string[] cardProps;
    }

    // Action describes an action that can be enqueued during the play phase.
    // This includes the card to be played for the action, its target(s), and
    // the time cost of the action.

    // Extension of List, used to model the sequence of actions created
    // during the play phase, and executed during the resolution phase.
    public class PlaySequence<T> : List<T> {
        public int totalTime;

        public PlaySequence() {
            totalTime = 0;
        }

        public int IndexOfCompleteTime(int targetTime) {
            for(int i = 0; i < this.Count; i++) {
                Action action = this[i] as Action;
                if(action.completeTime == targetTime) return i; 
            }
            return -1;
        }

        // adjusts the completeTime by amount speicfied in `offset` for each action starting with the specified `index`
        // this is used when dequeueing actions during play phase, and during resolution phase
        public void RecalculateCompleteTime(int index, int offset) {
            for(int i = index; i < this.Count; i++) {
                Action action = this[i] as Action;
                action.completeTime -= offset; 
            }
        }
        
        // helper function for enqueueing enemy actions between play and resolution phase
        public bool ContainsEnemyAction() {
            foreach(T entry in this) {
                if(entry is EnemyAction) return true;
            }
            return false;
        }

        public int GetLastEnemyActionTime() {
            int finalTime = 0;
            foreach(T entry in this) {
                if(entry is EnemyAction) {
                    EnemyAction enemyAction = entry as EnemyAction;
                    finalTime = enemyAction.completeTime;
                }
            }
            return finalTime;
        }

        public new void Add(T item) {
            base.Add(item);
            if(item.GetType() == typeof(PlayerAction)) {
                PlayerAction action = item as PlayerAction;
                this.totalTime += action.card.cost;
            }
        }

        public new void Remove(T item) {
            if(item.GetType() == typeof(PlayerAction)) {
                PlayerAction action = item as PlayerAction;
                int idx = this.IndexOfCompleteTime(action.completeTime);
                if(idx != -1) this.RecalculateCompleteTime(idx, action.card.cost);
                this.totalTime -= action.card.cost;
                action.card.curState = CardState.InHand;
                Board.me.Mulligan(action.card); // jank
                Destroy(action.instance);
            } else if (item is EnemyAction) {
                EnemyAction action = item as EnemyAction;
                Destroy(action.instance);
            }
            base.Remove(item);
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

    public IEnumerator ResetEnemySprites() {
        yield return new WaitForSeconds(.5f);
        foreach(GameObject enemy in enemies) enemy.GetComponent<SpriteRenderer>().sprite = enemy.GetComponent<Enemy>().combatStates[0];
    }

    public IEnumerator ResetPlayerSprites() {
        yield return new WaitForSeconds(.5f);
        player.GetComponent<SpriteRenderer>().sprite = player.GetComponent<Player>().combatStates[0];
    }
    public IEnumerator ExecuteAction(PlaySequence<Action> playSequence) {
        while(playSequence.Count != 0) {
            switch(playSequence[0].GetType().ToString()) {
                case "PlayerAction":
                    PlayerAction playerAction = playSequence[0] as PlayerAction;
                    playerAction.resolveAction();
                    yield return new WaitForSeconds(1f);
                    playSequence.Remove(playSequence[0]);
                    player.GetComponent<SpriteRenderer>().sprite = playerAction.card.cardProps[0] == "Attack" ? player.GetComponent<Player>().combatStates[1] : player.GetComponent<Player>().combatStates[2];
                    StartCoroutine(ResetPlayerSprites());
                    break;
                
                case "EnemyAction":
                    EnemyAction enemyAction = playSequence[0] as EnemyAction;
                    enemyAction.resolveAction();
                    yield return new WaitForSeconds(1f);
                    playSequence.Remove(playSequence[0]);
                    enemyAction.owner.GetComponent<SpriteRenderer>().sprite = enemyAction.owner.GetComponent<Enemy>().combatStates[(int)enemyAction.actionType + 1];
                    StartCoroutine(ResetEnemySprites());
                    break;
            }
        }
        if(borrowedTime != 0) GameObject.Find("HourglassGlow").GetComponent<HourglassGlow>().isActive = true;
        
    }

    public void MulToPlayPhase() {
        curPhase = Phase.Play;
        lockedHand.Clear();
        turn = 0;
        // FMOD Play Phase Transition Sound           
        toPlayPhaseSound.start();
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
        player = GameObject.Find("Player");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        List<CardData> deckList = LoadDeckData().deckList;
        GetAnchors(); // get anchor positions

        // initialize phase variables
        curPhase = Phase.Mulligan;
        mulLimit = 4;
        turn = 0;

        if(turn == 0) borrowedTime = 0;
        round = 0;
        
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
            cardScript.cardProps = card.cardProps;
            cardScript.cardArt = cardArtDict[cardScript.cardName]; 
            pool.Add(curCard);
        }
        pool = FisherYatesShuffle(pool);
        
        // now that all the preloading is done, actually put cards into the deck
        foreach(GameObject card in pool) {
            deck.Enqueue(card);
        }

        // FMOD object init
        lockSound = FMODUnity.RuntimeManager.CreateInstance(lockSoundEvent);
        toPlayPhaseSound = FMODUnity.RuntimeManager.CreateInstance(toPlayPhaseSoundEvent);
        toResolutionPhaseSound = FMODUnity.RuntimeManager.CreateInstance(toResolutionPhaseSoundEvent);
        toMulliganPhaseSound = FMODUnity.RuntimeManager.CreateInstance(toMulliganPhaseSoundEvent);
    }

    void Update(){
        actionButtonPressed = GameObject.FindObjectOfType<ActionButton>().buttonPressed;
        deckCount = deck.Count; // exposes variable for debug
        switch(curPhase){
            case Phase.Mulligan:
                while(hand.Count < 5){
                    DrawCard();
                }

                if(lockedHand.Count == 5 || mulLimit == 0) {
                    Invoke("MulToPlayPhase", .7f);
                    
                } else if(Input.GetKeyDown(KeyCode.E) || actionButtonPressed) {
                    turn++;
                    foreach(GameObject card in hand) {
                        if(!toMul.Contains(card) && !lockedHand.Contains(card)) {
                            lockedHand.Add(card);
                            // FMOD Play Lock Sound
                            lockSound.start();
                        }
                    }

                    foreach(GameObject card in toMul) {
                        Mulligan(card.GetComponent<Card>()); 
                    }
                    mulLimit = Mathf.Min(4 - turn, 4 - lockedHand.Count);
                    toMul.Clear();
                    GameObject.FindObjectOfType<ActionButton>().buttonPressed = false;
                }
                break;
            case Phase.Play:
                if(Input.GetKeyDown(KeyCode.E) || actionButtonPressed) {
                    // discard the cards that were not enqueue'd
                    foreach(GameObject card in hand) {
                        if(card.GetComponent<Card>().curState != CardState.InQueue) {
                            toMul.Add(card);
                        }
                    }
                    foreach(GameObject card in toMul) {
                        Mulligan(card.GetComponent<Card>()); 
                    }
                    toMul.Clear();

                    curPhase = Phase.Resolution;

                    // FMOD Resolution Phase Transition Sound
                    toResolutionPhaseSound.start();
                    // enqueue enemy actions
                    if (!playSequence.ContainsEnemyAction()) {
                        foreach(GameObject enemy in enemies) {
                            Enemy enemyScript = enemy.GetComponent<Enemy>();
                            foreach(EnemyAction actionToAdd in enemyScript.curActions) {
                                if(!playSequence.Contains(actionToAdd)) {
                                    int idx = playSequence.IndexOfCompleteTime(actionToAdd.completeTime);
                                    if(idx != -1) {
                                        playSequence.Insert(idx + 1, actionToAdd); // insert AFTER given index to give player priority in resolution
                                    } else {
                                        playSequence.Add(actionToAdd); // add to end if the scheduled play time is after the last player action
                                    }
                                }
                            }
                            enemyScript.prevActions = enemyScript.curActions;
                            enemyScript.curActions.Clear();
                        }
                    }
                    
                    // calculate borrowed time for next round                    
                    borrowedTime = playSequence.totalTime - playSequence.GetLastEnemyActionTime();
                    Debug.Log($"borrowed time of {borrowedTime}");
                    GameObject.FindObjectOfType<ActionButton>().buttonPressed = false;
                    StartCoroutine(ExecuteAction(playSequence)); // resolve all enqueued actions
                }
                
                break;

            case Phase.Resolution:
                if(playSequence.Count == 0) {
                    mulLimit = 4;
                    round++;

                    // reset block
                    player.GetComponent<Target>().block = 0;
                    foreach(GameObject enemy in enemies) enemy.GetComponent<Target>().block = 0;

                    // reset sprites to default stances
                    foreach(GameObject enemy in enemies) enemy.GetComponent<SpriteRenderer>().sprite = enemy.GetComponent<Enemy>().combatStates[0];
                    player.GetComponent<SpriteRenderer>().sprite = player.GetComponent<Player>().combatStates[0];
                    // FMOD Mulligan Phase Transition Sound
                    toMulliganPhaseSound.start();
                    curPhase = Phase.Mulligan;
                }
                break;
        }
            
    }
}
