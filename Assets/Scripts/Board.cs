using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using DG.Tweening;
using TMPro;

public enum Phase { Mulligan, Play, Resolution, Event };

public class Board : MonoBehaviour {
    private string deckFileName = "deck.json";

    // "STATE" FIELDS //
    public Phase curPhase;
    public bool overlayActive; // if this is true, disable interactive elements
    private bool punishing; // true while punishment coroutine is running
    public int borrowedTime; // offset time carryover if overplay/underplay
    public int round; // the number of mul-play-res cycles
    public string prevResolvedAction;
    public int level; // the number of complete fights (4 in total - 3 fights, 1 boss)
    private IEnumerator co; // reference for starting the ExecuteAction coroutine - allows us to stop it

    // "ENTITY" FIELDS //
    public static Board me;
    public GameObject player;
    public GameObject enemySpawner;
    public GameObject phaseBanner;
    public Transform[] eventContainers;
    public GameObject[] enemies;
    public GameObject perspectiveCamera;
    public bool actionButtonPressed;
    

    // CARD MANIPULATING FIELDS //
    public GameObject cardPrefab;
    private List<GameObject> pool = new List<GameObject>();
    private Dictionary<string, Sprite> cardArtDict = new Dictionary<string, Sprite>();
    public Dictionary<string, Transform> cardAnchors = new Dictionary<string, Transform>();

    public List<GameObject> deck = new List<GameObject>();
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
    private bool displayingEvents = false;

    // EVENT PHASE VARIABLES //
    public List<GameObject> possibleEvents = new List<GameObject>();
    public List<GameObject> curEvents = new List<GameObject>();

    //Particle Systems
    public ParticleSystem TimelineResolutionPS;

    // FMOD variables
    private SoundManager sm = SoundManager.me;

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
        public string superclass;
    }

    // Extension of List, used to model the sequence of actions created
    // during the play phase, and executed during the resolution phase.
    public class PlaySequence<T> : List<T> {
        public int totalTime;

        public PlaySequence() {
            totalTime = 0;
        }

        // used to find index to insert enemy actions
        public int IndexOfCompleteTime(int targetTime) {
            for(int i = 0; i < this.Count; i++) {
                Action action = this[i] as Action;
                if(action.completeTime == targetTime) return i; 
                else if (action.completeTime > targetTime) {
                    return i-1; // returns at least -1 (i is never < 0)
                } 
            }
            return -2;
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

        public bool ContainsActionAtTime(int target) {
            foreach(T entry in this) {
                Action action = entry as Action;
                if(action.completeTime == target) return true;
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

        public void DequeuePlayerAction(T item) {
            if(item.GetType() == typeof(PlayerAction)) {
                // FMOD Play Dequeue Sound
                me.sm.PlaySound(me.sm.dequeueCardSound); // Why is this playing the GenericQueueSound instead of the DequeueSound?

                PlayerAction action = item as PlayerAction;
                action.card.target = null;
                action.card.action = null;
                int idx = this.IndexOfCompleteTime(action.completeTime);
                if(idx != -1) this.RecalculateCompleteTime(idx, action.card.cost);
                this.totalTime -= action.card.cost;
                action.card.curState = CardState.InHand;
                action.card.isSettled = false;
                Destroy(action.instance);
            }
            base.Remove(item);
        
        }

        public new string ToString() {
            string retStr = "";
            foreach(T entry in this) {
                if(entry is PlayerAction) {
                    PlayerAction action = entry as PlayerAction;
                    retStr += $"Player action of {action.card.cardName} at time {action.completeTime}\n";
                } else if (entry is EnemyAction) {
                    EnemyAction action = entry as EnemyAction;
                    retStr += $"Enemy action of {action.actionType} at time {action.completeTime}\n";
                }
            }
            return retStr;
        }   
    }
    
    public void Mulligan(Card card) {
        if(hand.Contains(card.gameObject)) {
            card.OnMulligan();
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
        
        GameObject curCard = deck[0];
        deck.RemoveAt(0);

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
            deck.Add(card);
            Card curCard = card.GetComponent<Card>();
            curCard.isSettled = false;
            curCard.curState = CardState.InDeck;
            curCard.transform.parent = cardAnchors["Deck Anchor"];
        }
        discard.Clear();
    }

    private IEnumerator ResetEnemySprites() {
        yield return new WaitForSeconds(.5f);
        foreach(GameObject enemy in enemies) {
            enemy.GetComponent<SpriteRenderer>().sprite = enemy.GetComponent<Enemy>().combatStates[0];
            // enemy.transform.position = enemy.GetComponent<Target>().startPos;
        }
    }

    private IEnumerator ResetPlayerSprites() {
        yield return new WaitForSeconds(.5f);
        player.GetComponent<SpriteRenderer>().sprite = player.GetComponent<Player>().combatStates[0];
        // player.transform.position = player.GetComponent<Target>().startPos;
    }

    private IEnumerator ResetActionCamera() {
        yield return new WaitForSeconds(.5f);
        perspectiveCamera.transform.DOLocalMove(new Vector3(0, 0, 2), .5f);
    }

    private IEnumerator ExecuteAction(PlaySequence<Action> playSequence) {
        if(playSequence.Count == 0) {
            yield return new WaitForSeconds(1f);
        }

        // move actors closer together (resets at end of coroutine)
        player.transform.DOMoveX(-4.5f, .5f);
        enemies[0].transform.DOMoveX(4.5f, .5f);
        // GameObject.Find("Main Camera").GetComponent<Camera>().cullingMask = 0;

        while(playSequence.Count != 0) {
            switch(playSequence[0].GetType().ToString()) {
                case "PlayerAction":
                    PlayerAction playerAction = playSequence[0] as PlayerAction;
                    playerAction.card.resolveAction();

                    // anims
                    TimelineResolutionPS.Play();
                    playSequence.Remove(playSequence[0]);
                    perspectiveCamera.transform.DOLocalMove(new Vector3(-3.5f, 0, 8), .5f);
                    
                    // player.transform.position = new Vector3(-3, player.transform.position.y, player.transform.position.z);
                    // enemies[0].transform.position = new Vector3(3, enemies[0].transform.position.y, enemies[0].transform.position.z);

                    // player.transform.DOMoveX(-1.6f, .5f).SetEase(Ease.OutExpo);
                    // enemies[0].transform.DOMoveX(1.6f, .5f).SetEase(Ease.OutExpo);
                    
                    // TODO: abstract this out
                    player.GetComponent<SpriteRenderer>().sprite = playerAction.card.cardProps[0] == "Attack" ? player.GetComponent<Player>().combatStates[1] : player.GetComponent<Player>().combatStates[2];
                    yield return new WaitForSeconds(.2f);

                    // StartCoroutine(ResetActionCamera());
                    StartCoroutine(ResetPlayerSprites());
                    prevResolvedAction = "PlayerAction"; // probably find a better way to do this later
                    yield return new WaitForSeconds(1.5f);
                    break;
                
                case "EnemyAction":
                    EnemyAction enemyAction = playSequence[0] as EnemyAction;
                    enemyAction.resolveAction();
                    
                    // anims
                    playSequence.Remove(playSequence[0]);
                    perspectiveCamera.transform.DOLocalMove(new Vector3(3.5f, 0, 8), .5f);
                    
                    // player.transform.position = new Vector3(-3, player.transform.position.y, player.transform.position.z);
                    // enemies[0].transform.position = new Vector3(3, enemies[0].transform.position.y, enemies[0].transform.position.z);
                    // player.transform.DOMoveX(-1.6f, .5f).SetEase(Ease.OutExpo);
                    // enemies[0].transform.DOMoveX(1.6f, .5f).SetEase(Ease.OutExpo);

                    enemyAction.owner.GetComponent<SpriteRenderer>().sprite = enemyAction.owner.GetComponent<Enemy>().combatStates[(int)enemyAction.actionType + 1];
                    yield return new WaitForSeconds(.2f);

                    StartCoroutine(ResetEnemySprites());
                    prevResolvedAction = "EnemyAction"; // probably find a better way to do this later
                    yield return new WaitForSeconds(1.5f);
                    break;
            }
        }
        player.transform.DOMoveX(-10, .5f);
        enemies[0].transform.DOMoveX(10, .5f);

        if(borrowedTime != 0) {
            GameObject.Find("HourglassGlow").GetComponent<HourglassGlow>().isActive = true;
            GameObject.Find("TimelineGlow").GetComponent<HourglassGlow>().isActive = true;
        }
    }

    private IEnumerator Punishment(List<EnemyAction> list) {
        SpriteRenderer overlay = GameObject.Find("_DarknessActionOverlay").GetComponent<SpriteRenderer>();
        player.GetComponent<SpriteRenderer>().sortingLayerName = "Above Darkness";

        // FMOD change mix to punishment mix
        sm = SoundManager.me;
        sm.PlaySound(sm.punishmentSnapshot);

        overlay.enabled = true;
        overlay.color = new Color(1f, 1f, 1f, 0f);
        DOTween.To(()=> overlay.color, x=> overlay.color = x, new Color(1f, 1f, 1f, .6f), 1.5f);
        yield return new WaitForSeconds(1.5f);

        // FMOD play punishment sound
        sm.PlaySound(sm.overplayPunishmentSound);

        player.GetComponentsInChildren<ParticleSystem>()[1].Play();
        player.GetComponent<Player>().health -= (int)(.25f * player.GetComponent<Player>().health);
        player.transform.Find("DamageText").GetComponent<TextMeshPro>().text = ((int)(.25f * player.GetComponent<Player>().health)).ToString();
        player.transform.Find("DamageText").GetComponent<TextMeshPro>().sortingLayerID = SortingLayer.NameToID("Above Darkness");
        player.transform.Find("DamageText").GetComponent<DamageText>().FadeText();
        Camera.main.transform.DOShakePosition(2f);
        yield return new WaitForSeconds(2.0f);

        // FMOD change mix to battle mix
        sm.PlaySound(sm.battleSnapshot);

        overlay.enabled = false;
        foreach(EnemyAction action in list) {
            action.completeTime = action.baseCompleteTime;
            float xPos = Mathf.Max(0, action.completeTime * 1.14f);
            action.instance.transform.DOLocalMove(new Vector3(xPos, .98f, 0), .2f);
        }
        borrowedTime = 0;
        player.GetComponent<SpriteRenderer>().sortingLayerName = "Targets";
        player.transform.Find("DamageText").GetComponent<TextMeshPro>().sortingLayerID = SortingLayer.NameToID("Targets");
        punishing = false;
    }

    private IEnumerator DisplayEvents() {
        Debug.Log("hit coroutine");
        displayingEvents = true;
        yield return new WaitForSeconds(1.75f);
        curPhase = Phase.Event;
        playSequence.Clear();
        StartCoroutine(ResetActionCamera());
        StartCoroutine(ResetPlayerSprites());
        player.transform.DOMoveX(-10, .5f);
        GameObject actionManager = GameObject.Find("Actions");
        foreach(Transform child in actionManager.transform) {
            Destroy(child.gameObject);
        }

        phaseBanner.GetComponent<PhaseBanner>().canBanner = false;

        GameObject overlay = GameObject.Find("_DarknessOverlay");
        overlay.GetComponent<SpriteRenderer>().enabled = true; // enable without disabling input
        
        
        int doNotInclude = UnityEngine.Random.Range(0, possibleEvents.Count);
        int curEvent = 0;
        for(int i = 0; i < possibleEvents.Count; i++) {       
            if(i != doNotInclude) {
                Transform toAttach;
                foreach(Transform container in eventContainers) {
                    if(container.childCount == 0) {
                        toAttach = container;
                        Instantiate(possibleEvents[i], toAttach, false);
                        break;
                    }
                }
                curEvent++;
            } 
        }
        displayingEvents = false;
    }
    
    private void MulToPlayPhase() {  
        phaseBanner.GetComponent<PhaseBanner>().phaseName.text = "Play Phase";
        phaseBanner.GetComponent<PhaseBanner>().canBanner = true;
        phaseBanner.GetComponent<PhaseBanner>().doBanner();

        lockedHand.Clear();
        turn = 0;
        // FMOD Play Phase Transition Sound      
        sm = SoundManager.me;
        sm.PlaySound(sm.toPlayPhaseSound);
        curPhase = Phase.Play;
    }

    private void ResToMulPhase() {
        prevResolvedAction = "";
        mulLimit = 4;
        round++;

        phaseBanner.GetComponent<PhaseBanner>().phaseName.text = "Mulligan Phase"; 
        phaseBanner.GetComponent<PhaseBanner>().canBanner = true;
        phaseBanner.GetComponent<PhaseBanner>().doBanner();

        perspectiveCamera.transform.DOLocalMove(new Vector3(0, 0, 2), .5f);

        // reset block values
        player.GetComponent<Target>().block = 0;
        foreach(GameObject enemy in enemies) enemy.GetComponent<Target>().block = 0;

        // FMOD Mulligan Phase Transition Sound
        sm = SoundManager.me;
        sm.PlaySound(sm.toMulliganPhaseSound);
        curPhase = Phase.Mulligan;
    }

    public void EventToMulPhase() {
        // disable dark overlay
        GameObject.Find("_DarknessOverlay").GetComponent<SpriteRenderer>().enabled = false;

        // show mulligan banner
        GameObject phaseBanner = GameObject.Find("PhaseBanner"); 
        phaseBanner.GetComponent<PhaseBanner>().phaseName.text = "Mulligan Phase"; 
        phaseBanner.GetComponent<PhaseBanner>().canBanner = true;
        phaseBanner.GetComponent<PhaseBanner>().doBanner();

        // reset state variables  
        mulLimit = 4;
        turn = 0;
        borrowedTime = 0;
        round = 0;
        Reshuffle();
        level++;

        // spawn new enemies
        GameObject enemySpawner = GameObject.Find("EnemySpawner");
        EnemySpawner spawner = enemySpawner.GetComponent<EnemySpawner>();
        if(level != 4) {
            for(int i = 0; i < level; i++) {
                GameObject enemy = Instantiate(spawner.enemyList[UnityEngine.Random.Range(0, spawner.enemyList.Length)], enemySpawner.transform, false);
                enemy.transform.localPosition = new Vector3(i * -4.25f, 0, 9.3f);
            }
        } else {
            Instantiate(spawner.boss, new Vector3(0, 0, 9.3f), Quaternion.identity, enemySpawner.transform);
        }
        // GameObject.Find("Actions").GetComponent<ActionRenderer>().LateStart();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // destroy the event objects
        foreach(Transform container in eventContainers) {
            Destroy(container.GetComponentInChildren<Event>().gameObject);
        }
        curPhase = Phase.Mulligan;
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

    public List<GameObject> FisherYatesShuffle(List<GameObject> list) {
        for (int i = 0; i < list.Count; i++) {
            GameObject temp = list[i];
            int randIdx = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randIdx];
            list[randIdx] = temp;
        }
        return list;
    }

    private bool ContainsNonZeroActionCompleteTime(List<EnemyAction> list) {
        foreach(EnemyAction action in list) {
            if(action.completeTime > 0) return true;
        }
        return false;
    }

    private bool AllEnemiesDead() {
        if(enemies.Length == 0) return false;

        foreach(GameObject enemy in enemies) {
            if(enemy.GetComponent<Enemy>().health > 0) return false;
        }
        return true;
    }

    void Awake(){
        me = this;
    }

    void Start(){
        player = GameObject.Find("Player");
        enemySpawner = GameObject.Find("EnemySpawner");
        GameObject enemy = Instantiate(enemySpawner.GetComponent<EnemySpawner>().enemyList[0], enemySpawner.transform, false);

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        phaseBanner = GameObject.Find("PhaseBanner");
        perspectiveCamera = GameObject.Find("Perspective Camera");
        eventContainers = GameObject.Find("_EventManager").GetComponentsInChildren<Transform>();
        
        List<CardData> deckList = LoadDeckData().deckList;
        GetAnchors(); // get anchor positions

        // initialize phase variables
        curPhase = Phase.Mulligan;
        mulLimit = 4;
        turn = 0;

        if(turn == 0) borrowedTime = 0;
        round = 0;
        level = 1;

        foreach(CardData card in deckList){
            // load card art into dictionary
            string path = "file://" + Path.Combine(Application.streamingAssetsPath, card.artPath);
            LoadCardArt(path, card.cardName);

            // create card gameobject and populate its properties
            GameObject curCard = Instantiate(cardPrefab, cardAnchors["Deck Anchor"].position, Quaternion.identity);
            curCard.transform.parent = cardAnchors["Deck Anchor"];

            // if the string in `card.superclass` isn't a valid `Type`, add generic `Card` component to the card gameobject
            try {
                curCard.AddComponent(Type.GetType(card.superclass, true));
            } catch(Exception e) {
                curCard.AddComponent<Card>();
            }

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
            deck.Add(card);
        }
    }

    void Update(){
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        actionButtonPressed = GameObject.FindObjectOfType<ActionButton>().buttonPressed;
        deckCount = deck.Count; // exposes variable for debug
        if((AllEnemiesDead()) && (curPhase != Phase.Event && curPhase != Phase.Mulligan) && !displayingEvents) {
            // stop any ongoing coroutines/actions
            StopCoroutine(co);
            StartCoroutine(DisplayEvents());
            
        }

        switch(curPhase){
            case Phase.Mulligan:
                StartCoroutine(ResetEnemySprites());
                StartCoroutine(ResetPlayerSprites());               

                while(hand.Count < 5){
                    DrawCard();
                }

                if(lockedHand.Count == 5 || mulLimit == 0) {
                    if(!IsInvoking()) Invoke("MulToPlayPhase", .7f);
                    
                } else if(Input.GetKeyDown(KeyCode.E) || actionButtonPressed) {
                    turn++;
                    foreach(GameObject card in hand) {
                        if(!toMul.Contains(card) && !lockedHand.Contains(card)) {
                            lockedHand.Add(card);
                            // FMOD Play Lock Sound
                            sm = SoundManager.me;
                            sm.PlaySound(sm.lockSound);
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
                // check for Punishment mechanic conditions
                List<EnemyAction> enemyActions = new List<EnemyAction>();
                foreach(GameObject enemy in enemies) {
                    foreach(EnemyAction action in enemy.GetComponent<Enemy>().curActions) {
                        enemyActions.Add(action);
                    }
                }
                
                if(!ContainsNonZeroActionCompleteTime(enemyActions) && !punishing) {
                    StartCoroutine(Punishment(enemyActions));
                    punishing = true;
                }

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
                    sm = SoundManager.me;
                    sm.PlaySound(sm.toResolutionPhaseSound);
                    // enqueue enemy actions
                    if (!playSequence.ContainsEnemyAction()) {
                        foreach(GameObject enemy in enemies) {
                            Enemy enemyScript = enemy.GetComponent<Enemy>();
                            foreach(EnemyAction actionToAdd in enemyScript.curActions) {
                                if(!playSequence.Contains(actionToAdd)) {
                                    int idx = playSequence.IndexOfCompleteTime(actionToAdd.completeTime);
                                    if(actionToAdd.completeTime == 0 || idx == -1) {
                                        playSequence.Insert(0, actionToAdd);
                                    } else if(idx == -2) {
                                        playSequence.Add(actionToAdd); // add to end if the scheduled play time is after the last player action
                                    } else {
                                        playSequence.Insert(idx + 1, actionToAdd); // insert AFTER given index to give player priority in resolution
                                    }
                                }
                            }
                            enemyScript.prevActions = enemyScript.curActions;
                            enemyScript.curActions.Clear();
                        }
                    }
                    Debug.Log($"Play sequence is: \n{playSequence.ToString()}");

                    // calculate borrowed time for next round                    
                    borrowedTime = playSequence.totalTime - playSequence.GetLastEnemyActionTime();
                    GameObject.FindObjectOfType<ActionButton>().buttonPressed = false;
                    co = ExecuteAction(playSequence);
                    StartCoroutine(co); // resolve all enqueued actions
                }
                
                break;

            case Phase.Resolution:
                // waits for ExecuteAction coroutine to finish
                if(playSequence.Count == 0) {
                    if(!IsInvoking()) Invoke("ResToMulPhase", .7f);
                }
                break;
        }
            
    }
}
