using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public IEnumerator eventCo; 
    private bool displayingEvents = false;
    public bool displayingLoseScreen = false;

    // "ENTITY" FIELDS //
    public static Board instance;
    public GameObject player;
    public GameObject enemySpawner;
    public GameObject phaseBanner;
    public Transform[] eventContainers;
    public GameObject[] enemies;
    public GameObject perspectiveCamera;
    public bool actionButtonPressed;
    private SpriteRenderer[] daytimeSprites;
    private SpriteRenderer[] nighttimeSprites;
    public GameObject loseScreen;
    public GameObject winScreen;

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
    public int mulLimit;
    public List<GameObject> toMul = new List<GameObject>(); // is a subset of `hand`
    public List<GameObject> lockedHand = new List<GameObject>(); // also subset of `hand`, union with `toMul` is equal to `hand`

    // PLAY PHASE VARIABLES //
    public PlaySequence<Action> playSequence = new PlaySequence<Action>();
    

    // RESOLUTION PHASE VARIABLES //
    List<GameObject> elementsToTween = new PlaySequence<GameObject>();

    // EVENT PHASE VARIABLES //
    public List<GameObject> possibleEvents = new List<GameObject>();
    public List<GameObject> curEvents = new List<GameObject>();
    public float spawnEnemiesAtHealth;

    //Particle Systems
    public ParticleSystem TimelineResolutionPS;

    // FMOD variables
    public GameObject soundManagerPrefab;
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
            if(item is PlayerAction) {
                PlayerAction action = item as PlayerAction;
                foreach(T entry in this) {
                    if(entry is EnemyAction) {
                        EnemyAction toMove = entry as EnemyAction;
                        toMove.instance.transform.DOLocalMoveX((toMove.completeTime - action.completeTime) * 1.14f, .2f);
                    }
                }
                int idx = this.IndexOfCompleteTime(action.completeTime);
                if(idx != -1) this.RecalculateCompleteTime(idx, action.card.cost);
                this.totalTime -= action.card.cost;
                action.card.curState = CardState.InHand;
                Board.instance.Mulligan(action.card, false); // jank
                Destroy(action.instance);
            } else if (item is EnemyAction) {
                EnemyAction action = item as EnemyAction;
                Board.instance.DestroyActionInstance(action.instance);
            }
            base.Remove(item);
        }

        public void DequeuePlayerAction(T item) {
            if(item.GetType() == typeof(PlayerAction)) {
                // FMOD Play Dequeue Sound
                instance.sm.PlaySound(instance.sm.dequeueCardSound); // Why is this playing the GenericQueueSound instead of the DequeueSound?

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

    private void DestroyActionInstance(GameObject actionInstance) {
        StartCoroutine(actionInstance.GetComponent<EnemyIntentRenderer>().DestroyEnemyAction(actionInstance));
    }
    
    public void Mulligan(Card card, bool useEffect) {
        if(hand.Contains(card.gameObject)) {
            if(useEffect) card.OnMulligan();
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
        Card cardScript = curCard.GetComponent<Card>();
        deck.RemoveAt(0);
        // foreach(SpriteRenderer sr in cardScript.cardParts) {
        //     if(sr != cardScript.cardParts[3] && sr != cardScript.cardParts[5]) sr.enabled = true;
        // }
        cardScript.isSettled = false;
        curCard.GetComponent<Card>().curState = CardState.InHand;
        
        hand.Add(curCard);
        // find empty hand anchor
        for(int i = 0; i < 5; i++) {
            Transform anchor = GameObject.Find($"Hand{i}").transform;
                if(anchor.childCount == 0){
                    curCard.transform.parent = anchor;
                }
        }

        curCard.GetComponent<Card>().OnDraw();
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

        foreach(GameObject go in elementsToTween) {
            go.transform.DOMoveY(go.transform.position.y - 2f, .75f);
        }

        // move actors closer together (resets at end of coroutine)
        player.transform.DOMoveX(-4.5f - (.85f * (enemies.Length - 1)), .5f);
        for(int i = enemies.Length - 1; i >= 0; i--) {
            enemies[i].transform.DOLocalMoveX(-4.5f - (3.7f * i), .5f - (.05f * i)); // DO IT BACK AT END
        }

        while(playSequence.Count != 0) {
            switch(playSequence[0].GetType().ToString()) {
                case "PlayerAction":
                    PlayerAction playerAction = playSequence[0] as PlayerAction;
                    playerAction.card.ResolveAction();

                    // anims
                    TimelineResolutionPS.Play();
                    playSequence.Remove(playSequence[0]);
                    perspectiveCamera.transform.DOLocalMove(new Vector3(-2f, 0, 8), .5f);
                    
                    // player.transform.position = new Vector3(-3, player.transform.position.y, player.transform.position.z);
                    // enemies[0].transform.position = new Vector3(3, enemies[0].transform.position.y, enemies[0].transform.position.z);

                    // player.transform.DOMoveX(-1.6f, .5f).SetEase(Ease.OutExpo);
                    // enemies[0].transform.DOMoveX(1.6f, .5f).SetEase(Ease.OutExpo);
                    
                    // TODO: abstract this out
                    // player.GetComponent<SpriteRenderer>().sprite = playerAction.card.cardProps[0] == "Attack" ? player.GetComponent<Player>().combatStates[1] : player.GetComponent<Player>().combatStates[2];
                    yield return new WaitForSeconds(.2f);

                    // StartCoroutine(ResetActionCamera());
                    StartCoroutine(ResetPlayerSprites());
                    prevResolvedAction = "PlayerAction"; // probably find a better way to do this later
                    yield return new WaitForSeconds(1.5f);
                    break;
                
                case "EnemyAction":
                    EnemyAction enemyAction = playSequence[0] as EnemyAction;
                    enemyAction.ResolveAction();
                    
                    // anims
                    playSequence.Remove(playSequence[0]);
                    perspectiveCamera.transform.DOLocalMove(new Vector3(2f, 0, 8), .5f);
                    
                    // player.transform.position = new Vector3(-3, player.transform.position.y, player.transform.position.z);
                    // enemies[0].transform.position = new Vector3(3, enemies[0].transform.position.y, enemies[0].transform.position.z);
                    // player.transform.DOMoveX(-1.6f, .5f).SetEase(Ease.OutExpo);
                    // enemies[0].transform.DOMoveX(1.6f, .5f).SetEase(Ease.OutExpo);

                    // enemyAction.owner.GetComponent<SpriteRenderer>().sprite = enemyAction.owner.GetComponent<Enemy>().combatStates[(int)enemyAction.actionType + 1];
                    yield return new WaitForSeconds(.2f);

                    StartCoroutine(ResetEnemySprites());
                    prevResolvedAction = "EnemyAction"; // probably find a better way to do this later
                    yield return new WaitForSeconds(1.5f);
                    break;
            }
                
        }

        // move actors and ui elements back to position
        player.transform.DOMoveX(-10, .5f);
        for(int i = 0; i < enemies.Length; i++) {
            enemies[i].transform.DOLocalMoveX(i * - 5.5f, .5f);
        }
        
        foreach(GameObject go in elementsToTween) {
            go.transform.DOMoveY(go.transform.position.y + 2f, .75f);
        }

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
        player.GetComponent<Player>().health -= (int)(.5f * player.GetComponent<Player>().health);
        player.transform.Find("DamageText").GetComponent<TextMeshPro>().text = ((int)(.5f * player.GetComponent<Player>().health)).ToString();
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
        StartCoroutine(UnityEngine.Object.FindObjectOfType<DarkProgressBar>().AdjustForBorrowedTime());
        borrowedTime = 0;
        player.GetComponent<SpriteRenderer>().sortingLayerName = "Targets";
        player.transform.Find("DamageText").GetComponent<TextMeshPro>().sortingLayerID = SortingLayer.NameToID("Targets");
        punishing = false;
    }

    private IEnumerator DisplayLoseScreen() {
        displayingLoseScreen = true;
        StartCoroutine(ResetActionCamera());
        for(int i = 0; i < enemies.Length; i++) {
            enemies[i].transform.DOLocalMoveX(i * - 5.5f, .5f);
        }

        player.transform.Find("HealthBarBase").gameObject.SetActive(false);
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        player.transform.DOShakePosition(1f, .75f);
        yield return new WaitForSeconds(.25f);
        DOTween.To(()=> sr.color, x=> sr.color = x, new Color(sr.color.r, sr.color.g, sr.color.b, 0), 1.5f);
        player.transform.Find("BasicShadow").gameObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);

        SpriteRenderer overlay = GameObject.Find("_DarknessOverlay").GetComponent<SpriteRenderer>();
        overlay.enabled = true;
        overlay.color = new Color(1f, 1f, 1f, 0f);
        DOTween.To(()=> overlay.color, x=> overlay.color = x, new Color(1f, 1f, 1f, .75f), 1.5f);
        yield return new WaitForSeconds(1.5f);
        
        Destroy(player.gameObject);
        loseScreen.SetActive(true);
    }

    public IEnumerator DisplayWinScreen() {
        StopCoroutine(co);
        displayingLoseScreen = true;
        StartCoroutine(ResetActionCamera());
        player.transform.DOMoveX(-10, .5f);
        
        SpriteRenderer overlay = GameObject.Find("_DarknessOverlay").GetComponent<SpriteRenderer>();
        overlay.enabled = true;
        overlay.color = new Color(1f, 1f, 1f, 0f);
        DOTween.To(()=> overlay.color, x=> overlay.color = x, new Color(1f, 1f, 1f, .75f), 1.5f);
        yield return new WaitForSeconds(1.5f);
        winScreen.SetActive(true);
    }

    private IEnumerator DisplayEvents() {
        displayingEvents = true;
        for(int i = hand.Count - 1; i >= 0; i--) {
            Mulligan(hand[i].GetComponent<Card>(), false);
        }
        yield return new WaitForSeconds(1.5f);

        // FMOD set parameter for ambience to 1 (night)
        sm = SoundManager.me;
        sm.SetSoundParameter(sm.ambienceSound, "DayNight", 1f);

        // for some reason DOTween doesn't like my for loop so we have this monstrosity instead
        DOTween.To(()=> daytimeSprites[0].color, x=> daytimeSprites[0].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> nighttimeSprites[0].color, x=> nighttimeSprites[0].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> daytimeSprites[1].color, x=> daytimeSprites[1].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> nighttimeSprites[1].color, x=> nighttimeSprites[1].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> daytimeSprites[2].color, x=> daytimeSprites[2].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> nighttimeSprites[2].color, x=> nighttimeSprites[2].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> daytimeSprites[3].color, x=> daytimeSprites[3].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> nighttimeSprites[3].color, x=> nighttimeSprites[3].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> daytimeSprites[4].color, x=> daytimeSprites[4].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> nighttimeSprites[4].color, x=> nighttimeSprites[4].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> daytimeSprites[0].color, x=> daytimeSprites[5].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> nighttimeSprites[0].color, x=> nighttimeSprites[5].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> daytimeSprites[0].color, x=> daytimeSprites[6].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> nighttimeSprites[0].color, x=> nighttimeSprites[6].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> daytimeSprites[0].color, x=> daytimeSprites[7].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> nighttimeSprites[0].color, x=> nighttimeSprites[7].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> daytimeSprites[0].color, x=> daytimeSprites[8].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> nighttimeSprites[0].color, x=> nighttimeSprites[8].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> daytimeSprites[0].color, x=> daytimeSprites[9].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> nighttimeSprites[0].color, x=> nighttimeSprites[9].color = x, new Color32(255, 255, 255, 255), 2.00f);

        // for(int i = 0; i < daytimeSprites.Length; i++) {
        //     DOTween.To(()=> daytimeSprites[i].color, x=> daytimeSprites[i].color = x, new Color32(255, 255, 255, 0), 2.00f);
        //     DOTween.To(()=> nighttimeSprites[i].color, x=> nighttimeSprites[i].color = x, new Color32(255, 255, 255, 255), 2.00f);
        // }

        curPhase = Phase.Event;
        
        StartCoroutine(ResetActionCamera());
        StartCoroutine(ResetPlayerSprites());
        player.transform.DOMoveX(-10, .5f);
        yield return new WaitForSeconds(.5f);
        foreach(GameObject go in elementsToTween) {
            go.transform.DOMoveY(go.transform.position.y + 2f, .75f);
        }

        // reset playSequence
        foreach(Action action in playSequence) {
            if(action is PlayerAction) {
                PlayerAction playerAction = action as PlayerAction;
                playerAction.card.curState = CardState.InHand;
                Board.instance.Mulligan(playerAction.card, false);
                Destroy(action.instance);
            } else if (action is EnemyAction) {
                EnemyAction enemyAction = action as EnemyAction;
                Destroy(enemyAction.instance);
            }
        }
        playSequence.Clear();
        playSequence.totalTime = 0;

        GameObject actionManager = GameObject.Find("Actions");
        foreach(Transform child in actionManager.transform) {
            Destroy(child.gameObject);
        }
        yield return new WaitForSeconds(1.75f);

        GameObject overlay = GameObject.Find("_DarknessOverlay");
        SpriteRenderer overlaySr = overlay.GetComponent<SpriteRenderer>();
        overlaySr.enabled = true; // enable without disabling input     
        
        int doNotInclude = UnityEngine.Random.Range(0, possibleEvents.Count);
        int curEvent = 0;
        for(int i = 0; i < possibleEvents.Count; i++) {       
            // if(i != doNotInclude) {
                Transform toAttach;
                foreach(Transform container in eventContainers) {
                    if(container.childCount == 0) {
                        toAttach = container;
                        Instantiate(possibleEvents[i], toAttach, false);
                        break;
                    }
                }
                curEvent++;
            // } 
        }
        displayingEvents = false;
    }
    
    private void MulToPlayPhase() {  
        phaseBanner.GetComponent<PhaseBanner>().phaseName.text = "Play Phase";
        phaseBanner.GetComponent<PhaseBanner>().canBanner = true;
        phaseBanner.GetComponent<PhaseBanner>().doBanner();
        GameObject.Find("Actions").GetComponent<ActionRenderer>().adjusted = false;
        lockedHand.Clear();
        // FMOD Play Phase Transition Sound      
        sm = SoundManager.me;
        sm.PlaySound(sm.toPlayPhaseSound);
        curPhase = Phase.Play;
    }

    private void ResToMulPhase() {
        if(displayingEvents || displayingLoseScreen) return;
        prevResolvedAction = "";
        mulLimit = 5;
        Card.charged = false;
        round++;

        // if(!displayingEvents && !displayingLoseScreen) {
            phaseBanner.GetComponent<PhaseBanner>().phaseName.text = "Mulligan Phase"; 
            phaseBanner.GetComponent<PhaseBanner>().canBanner = true;
            phaseBanner.GetComponent<PhaseBanner>().doBanner();
        // }

        perspectiveCamera.transform.DOLocalMove(new Vector3(0, 0, 2), .5f);

        // reset block values
        player.GetComponent<Target>().block = 0;
        foreach(GameObject enemy in enemies) enemy.GetComponent<Target>().block = 0;

        // FMOD Mulligan Phase Transition Sound
        sm = SoundManager.me;
        sm.PlaySound(sm.toMulliganPhaseSound);
        curPhase = Phase.Mulligan;
    }

    public IEnumerator EventToMulPhase() {
        GameObject.Find("_DarknessOverlay").GetComponent<SpriteRenderer>().enabled = false;
        foreach(Transform container in eventContainers) {
            Transform evt = container.GetComponentInChildren<Event>().transform;
            evt.GetComponent<SpriteRenderer>().enabled = false;
            evt.GetComponentsInChildren<TextMeshPro>()[0].enabled = false;
            evt.GetComponentsInChildren<TextMeshPro>()[1].enabled = false;
        }

        // FMOD set parameter for ambience to 0 (day)
        sm = SoundManager.me;
        sm.SetSoundParameter(sm.ambienceSound, "DayNight", 0f);

        // transition to day
        DOTween.To(()=> daytimeSprites[0].color, x=> daytimeSprites[0].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> nighttimeSprites[0].color, x=> nighttimeSprites[0].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> daytimeSprites[1].color, x=> daytimeSprites[1].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> nighttimeSprites[1].color, x=> nighttimeSprites[1].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> daytimeSprites[2].color, x=> daytimeSprites[2].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> nighttimeSprites[2].color, x=> nighttimeSprites[2].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> daytimeSprites[3].color, x=> daytimeSprites[3].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> nighttimeSprites[3].color, x=> nighttimeSprites[3].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> daytimeSprites[4].color, x=> daytimeSprites[4].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> nighttimeSprites[4].color, x=> nighttimeSprites[4].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> daytimeSprites[0].color, x=> daytimeSprites[5].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> nighttimeSprites[0].color, x=> nighttimeSprites[5].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> daytimeSprites[0].color, x=> daytimeSprites[6].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> nighttimeSprites[0].color, x=> nighttimeSprites[6].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> daytimeSprites[0].color, x=> daytimeSprites[7].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> nighttimeSprites[0].color, x=> nighttimeSprites[7].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> daytimeSprites[0].color, x=> daytimeSprites[8].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> nighttimeSprites[0].color, x=> nighttimeSprites[8].color = x, new Color32(255, 255, 255, 0), 2.00f);
        DOTween.To(()=> daytimeSprites[0].color, x=> daytimeSprites[9].color = x, new Color32(255, 255, 255, 255), 2.00f);
        DOTween.To(()=> nighttimeSprites[0].color, x=> nighttimeSprites[9].color = x, new Color32(255, 255, 255, 0), 2.00f);
        
        // for(int i = 0; i < daytimeSprites.Length; i++) {
        //     DOTween.To(() => daytimeSprites[i].color, x => daytimeSprites[i].color = x, new Color32(255, 255, 255, 255), 2.00f);
        //     DOTween.To(() => nighttimeSprites[i].color, x => nighttimeSprites[i].color = x, new Color32(255, 255, 255, 0), 2.00f);
        // }
        yield return new WaitForSeconds(1.5f);

        // show mulligan banner
        GameObject phaseBanner = GameObject.Find("PhaseBanner"); 
        phaseBanner.GetComponent<PhaseBanner>().phaseName.text = "Mulligan Phase"; 
        phaseBanner.GetComponent<PhaseBanner>().canBanner = true;
        phaseBanner.GetComponent<PhaseBanner>().doBanner();

        // reset state variables
        playSequence.totalTime = 0;
        player.GetComponent<Player>().block = 0;
        mulLimit = 5;
        borrowedTime = 0;
        GameObject.Find("HourglassGlow").GetComponent<HourglassGlow>().isActive = false;
        GameObject.Find("TimelineGlow").GetComponent<HourglassGlow>().isActive = false;
        round = 0;
        Reshuffle();
        level++;
        foreach(GameObject card in deck) {
            card.GetComponent<Card>().OnNewCombat();
            card.GetComponent<Card>().curState = CardState.InDeck;
        }

        // spawn new enemies
        EnemySpawner spawner = enemySpawner.GetComponent<EnemySpawner>();
        switch(level) {
            case 1:
                spawner.SpawnEnemy(0, 0);
                break;
            case 2:
                spawner.SpawnEnemy(0, 0);
                spawner.SpawnEnemy(1, 1);
                break;
            case 3:
                spawner.SpawnEnemy(2, 0);
                break;
            case 4:
                spawner.SpawnEnemy(1, 0);
                spawner.SpawnEnemy(2, 1);
                break;
            // boss
            case 5:
                GameObject enemy = Instantiate(enemySpawner.GetComponent<EnemySpawner>().boss, enemySpawner.transform, false);
                enemy.GetComponent<Enemy>().health = (int)(enemy.GetComponent<Enemy>().maxHealth * spawnEnemiesAtHealth);
                break;
        }
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // destroy the event objects
        foreach(Transform container in eventContainers) {
            Destroy(container.GetComponentInChildren<Event>().gameObject);
        }

        spawnEnemiesAtHealth = 1f;
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

    void Awake() {
        // Spawn FMOD sound manager if it doesn't exist.
        if (GameObject.Find("_SoundManager")) { }
        else
            Instantiate(soundManagerPrefab);
        instance = this;
    }

    void Start() {
        player = GameObject.Find("Player");
        enemySpawner = GameObject.Find("EnemySpawner");
        GameObject enemy = Instantiate(enemySpawner.GetComponent<EnemySpawner>().enemyList[0], enemySpawner.transform, false);
        enemy.GetComponent<Enemy>().health = enemy.GetComponent<Enemy>().maxHealth;

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        phaseBanner = GameObject.Find("PhaseBanner");
        perspectiveCamera = GameObject.Find("Perspective Camera");
        eventContainers = GameObject.Find("_EventManager").GetComponentsInChildren<Transform>();
        daytimeSprites = GameObject.Find("DaytimeBackground").GetComponentsInChildren<SpriteRenderer>();
        nighttimeSprites = GameObject.Find("NighttimeBackground").GetComponentsInChildren<SpriteRenderer>();
        spawnEnemiesAtHealth = 1f;

        // used to specify ui elements to tween down during res phase
        elementsToTween.Add(GameObject.Find("_HandAnchor"));
        elementsToTween.Add(GameObject.Find("Playfield"));
        elementsToTween.Add(GameObject.Find("_DiscardAnchor"));
        elementsToTween.Add(GameObject.Find("_DeckAnchor"));

        List<CardData> deckList = LoadDeckData().deckList;
        GetAnchors(); // get anchor positions

        // initialize phase variables
        curPhase = Phase.Mulligan;
        mulLimit = 5;

        borrowedTime = 0;
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

    void Update() {
        // debug shortcuts
        if(Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(2);
        if(Input.GetKeyDown(KeyCode.T)) {
            foreach(GameObject enemy in enemies) {
                enemy.GetComponent<Enemy>().health = 1;
            }
        }
        // if(Input.GetKeyDown(KeyCode.F)) player.GetComponent<Player>().health = 1;

        if(Input.GetMouseButtonDown(1)) {
            for(int i = playSequence.Count - 1; i >= 0; i--) {
                if(playSequence[i] is PlayerAction) {
                    playSequence.DequeuePlayerAction(playSequence[i]);
                    break;
                }
            }
        }

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        actionButtonPressed = GameObject.FindObjectOfType<ActionButton>().buttonPressed;

        if(!displayingLoseScreen && player.GetComponent<Player>().health <= 0) {
            StopCoroutine(co);
            StartCoroutine(DisplayLoseScreen());
        }

        if(level < 4 && (AllEnemiesDead()) && (curPhase != Phase.Event && curPhase != Phase.Mulligan) && !displayingEvents) {
            // stop any ongoing coroutines/actions
            StopCoroutine(co);
            eventCo = DisplayEvents();
            StartCoroutine(eventCo);
        }

        switch(curPhase){
            case Phase.Mulligan:
                GameObject.Find("MulCounter").GetComponent<TextMeshPro>().text = $"Redraws: {mulLimit}";
                StartCoroutine(ResetEnemySprites());
                StartCoroutine(ResetPlayerSprites());               

                while(hand.Count < 5) {
                    DrawCard();
                }

                if(Input.GetKeyDown(KeyCode.E) || actionButtonPressed) {
                    if(toMul.Count == 0) {
                        mulLimit = 0;
                    }
                    foreach(GameObject card in toMul) {
                        Mulligan(card.GetComponent<Card>(), true); 
                        sm = SoundManager.me;
                        sm.PlaySound(sm.lockSound);
                        mulLimit--;
                    }
                    GameObject.FindObjectOfType<ActionButton>().buttonPressed = false;
                    toMul.Clear();
                }

                if(mulLimit == 0) {
                    if(!IsInvoking()) Invoke("MulToPlayPhase", .7f);
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
                if(playSequence.Count == 0 && !AllEnemiesDead()) {
                    if(!IsInvoking()) Invoke("ResToMulPhase", .7f);
                }
                break;
        }       
    }
}
