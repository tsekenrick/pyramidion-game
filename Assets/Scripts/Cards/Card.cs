using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using DG.Tweening;
using TMPro;

public enum CardState {InDeck, InHand, InDiscard, InPlay, InQueue, InSelectionRemove, InSelectionAdd }; 

[System.Serializable]
public class Card : MonoBehaviour {

    public CardState curState;
    public static bool charged;
    private Board board = Board.instance;
    private SoundManager sm = SoundManager.me;

    public SpriteRenderer[] cardParts;
    public TextMeshPro[] textParts;
    [SerializeField]
    private Sprite[] cardSprites = new Sprite[3];

    // anim related fields
    private Sequence tweenSequence;
    private Transform tr;
    private Transform prevParent;
    public bool isSettled = true;
    private bool playingMul = false;
    protected bool shake = true;

    // fields read from json
    public string cardName;
    public int cost;
    public string desc;
    public Sprite cardArt;
    public string[] cardProps;

    public GameObject target; // null before card is "played"
    public PlayerAction action; // also null before card is played

    private IEnumerator DrawAnim(Transform tr) {
        GetComponent<BoxCollider2D>().enabled = true;
        foreach(SpriteRenderer sr in cardParts) sr.sortingLayerName = "UI Low";
        cardParts[5].sortingLayerName = "UI High";
        cardParts[4].sortingOrder = 3;

        foreach(TextMeshPro tmp in textParts) {
            tmp.enabled = true;
            tmp.sortingOrder = -1;
        }   

        tr.DOScale(Vector3.one, .3f);
        tr.DOMove(tr.parent.position, .3f);       
        yield return new WaitForSeconds(.3f);
        isSettled = true;

    }

    private void PlayAnim(Transform tr) {
        tr.localScale = Vector3.zero;
        tr.position = tr.parent.position;
        tr.localScale = Vector3.one;
    }

    public IEnumerator MulliganAnim(Transform tr) {
        playingMul = true;
        tr.DOMove(tr.parent.position, .3f);
        tr.DOScale(Vector3.zero, .3f);
        yield return new WaitForSeconds(.3f);
        foreach(SpriteRenderer sr in cardParts) sr.enabled = false; 
        foreach(TextMeshPro tmp in textParts) tmp.enabled = false;
        isSettled = true;
        playingMul = false;
    }

    private IEnumerator ReshuffleAnim(Transform tr) {
        cardParts[0].enabled = true;
        cardParts[2].enabled = true;

        tr.DOMove(tr.parent.position, .6f);
        tr.DOScale(.3f * Vector3.one, .3f);
        tr.DOScale(Vector3.zero, .3f);
        yield return new WaitForSeconds(.6f);
        foreach(SpriteRenderer sr in cardParts) sr.enabled = false;
        foreach(TextMeshPro tmp in textParts) tmp.enabled = false;
        isSettled = true;
    }

    public void Attack(int amount, GameObject target) {
        board.player.GetComponent<SpriteRenderer>().sprite = board.player.GetComponent<Player>().combatStates[1];

        Target t = target.GetComponentInParent<Target>();
        if(charged) {
            amount *= 2;
        }
        int tmpBlock = t.block;
        t.block = Mathf.Max(t.block - amount, 0);
        t.transform.Find("DamageText").GetComponent<TextMeshPro>().text = $"{Mathf.Max(amount - tmpBlock, 0)}";
        t.GetComponentInChildren<DamageText>().FadeText();
        t.health -= Mathf.Max(amount - tmpBlock, 0);

        if(Mathf.Max(amount - tmpBlock, 0) > 0) {
            t.transform.Find("TakingDamagePS").GetComponent<ParticleSystem>().Play();
            if(shake) Camera.main.transform.DOShakePosition(.5f);
        } else {
            t.transform.Find("DamagedShieldPS").GetComponent<ParticleSystem>().Play();
            if(shake) Camera.main.transform.DOShakePosition(.5f, .5f);
        }
    }

    public void Defend(int amount, GameObject target) {
        board.player.GetComponent<SpriteRenderer>().sprite = board.player.GetComponent<Player>().combatStates[2];
        Target t = board.player.GetComponent<Target>();
        t.transform.Find("ShieldPS").GetComponent<ParticleSystem>().Play();

        // currently doesn't work - will fix later
        // Sequence animShield = DOTween.Sequence();
        // animShield.Append(t.transform.Find("HealthBarBase").Find("BlockIcon").DOScale(2f, .25f));
        // animShield.Append(t.transform.Find("HealthBarBase").Find("BlockIcon").DOScale(1f, .25f));
        
        t.block += amount;
    }

    // utility functions to be implemented in superclasses
    public virtual void OnMulligan() {
        return;
    }

    public virtual void OnEnqueue() {
        return;
    }

    public virtual void OnDequeue() {
        return;
    }

    public virtual void OnDraw() {
        return;
    }

    public virtual void OnNewCombat() {
        return;
    }

    public virtual void ResolveAction() {
        MethodInfo mi = this.GetType().GetMethod(this.cardProps[0]);
        switch(cardProps[0]) {
            case "Attack":
                // FMOD Player Attack Sound
                sm = SoundManager.me;
                sm.PlayPlayerAttackSound();
                break;
            case "Defend":
                // FMOD Player Defend Sound
                sm = SoundManager.me;
                sm.PlayPlayerDefendSound();
                break;
        }
        mi.Invoke(this, new object[]{int.Parse(this.cardProps[1]), this.target});
        charged = false;
    }

    public virtual void Awake() {
        tweenSequence = DOTween.Sequence();
        cardParts = GetComponentsInChildren<SpriteRenderer>();
        textParts = GetComponentsInChildren<TextMeshPro>();
        tr = this.gameObject.transform;
        curState = CardState.InDeck;

        GetComponent<BoxCollider2D>().enabled = false;
        foreach(SpriteRenderer sr in cardParts) sr.enabled = false;
        foreach(TextMeshPro tmp in textParts) tmp.enabled = false;
    }

    public virtual void Update(){
        if((board.lockedHand.Contains(this.gameObject) || board.lockedHand.Count >= 4) && board.curPhase == Phase.Mulligan) {
            foreach(SpriteRenderer sr in cardParts) {
                if(sr != cardParts[4]) sr.color = new Color(.5f, .5f, .5f, 1f);
            }
            cardParts[4].enabled = false; // kill glow
        } else if(curState == CardState.InHand) {
            foreach(SpriteRenderer sr in cardParts) {
                if(sr != cardParts[4]) sr.color = Color.white;
                if(sr != cardParts[5] && sr != cardParts[3]) {
                    sr.enabled = true;
                }
                if(sr != cardParts[4] && sr != cardParts[5]) sr.sortingOrder = 6;
            }
            
            foreach(TextMeshPro tmp in textParts) {
                tmp.enabled = true;
                tmp.sortingLayerID = SortingLayer.NameToID("UI High");
            }
            cardParts[4].sortingOrder = 3;
        }

        cardParts[5].sortingLayerName = "UI High"; // mulligan X comes above all other card elements        
        if(curState != CardState.InQueue) {
            foreach(SpriteRenderer sr in cardParts){
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            }
        }
         
        // tween to the correct pile depending on state
        switch(curState) {
            case CardState.InHand:
                if(!isSettled) {
                    GetComponent<TrailRenderer>().enabled = true;
                    StartCoroutine(DrawAnim(tr));
                    for(int i = 0; i < 3; i++){
                        cardParts[i].enabled = true;
                        cardParts[1].sprite = cardArt;
                    }

                    textParts[0].text = cardName;
                    textParts[1].text = desc;
                    textParts[2].text = cost.ToString();

                    foreach (TextMeshPro tmp in textParts) {
                        tmp.sortingLayerID = SortingLayer.NameToID("UI High");
                    }
                    // FMOD Draw Event
                    sm.PlaySound(sm.drawSound);
                } else {
                    GetComponent<TrailRenderer>().enabled = false;
                }
                break;

            case CardState.InDiscard:
                if(!isSettled) {
                    GetComponent<TrailRenderer>().enabled = true;
                    StartCoroutine(MulliganAnim(tr));
                    
                    // FMOD Discard Event
                    sm.PlaySound(sm.discardSound);
                } else if(!playingMul) {
                    GetComponent<TrailRenderer>().enabled = false;
                }
                break;
            
            case CardState.InDeck:
                if(!isSettled) {
                    GetComponent<TrailRenderer>().enabled = true;
                    StartCoroutine(ReshuffleAnim(tr));
                    // FMOD Shuffle Event
                    sm.PlaySound(sm.shuffleSound);
                } else if(!playingMul) {
                    GetComponent<TrailRenderer>().enabled = false;
                    
                }
                break;
            
            case CardState.InPlay:
                // failsafe for if holding card when going from play to res
                if(board.curPhase != Phase.Play) {
                    tr.parent = prevParent;
                    prevParent = null;
                    foreach(TextMeshPro tmp in textParts) tmp.sortingOrder = 7;
                    curState = CardState.InHand;
                    isSettled = false; // initiates tween back to hand pos
                    tr.DOLocalMoveY(0f, .3f).SetDelay(.35f);
                }
                Vector3 mousePos = Input.mousePosition;
                tr.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
                tr.DOScale(.75f, .3f).SetId("PlayScale");
                break;

            case CardState.InQueue:
                if(!isSettled) {
                    tr.position = tr.parent.position;
                    DOTween.Pause("PlayScale");
                    tr.DOScale(1, .1f);
                    // PlayAnim(tr);
                    isSettled = true;
                    foreach(SpriteRenderer sr in cardParts){
                        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);
                        sr.sortingLayerName = "UI Low"; 
                    } 

                    foreach(TextMeshPro tmp in textParts) {
                        tmp.sortingLayerID = SortingLayer.NameToID("UI Low");
                    }
                    cardParts[4].enabled = false;
                }
                break;

        }
    }
    

    void OnMouseEnter(){ 
        if(board.overlayActive && board.curPhase != Phase.Event) return;

        if(curState == CardState.InHand) {
            if(board.lockedHand.Contains(this.gameObject)) return;

            foreach(SpriteRenderer sr in cardParts) {
                sr.sortingLayerName = "UI High";
                sr.sortingOrder = 6;
            }
            cardParts[5].sortingOrder = 8;
            cardParts[4].sortingOrder = 3; // set glow below the rest
            foreach(TextMeshPro tmp in textParts) tmp.sortingOrder = 7;
            tweenSequence.Append(tr.DOScale(1.4f * Vector3.one, .25f).SetId("zoomIn"));
            //tweenSequence.Insert(0, tr.DOMoveZ(-1f, .5f).SetId("zoomIn"));
            // FMOD Hover Event
            sm.PlaySound(sm.hoverSound);
        } else if(curState == CardState.InSelectionRemove || curState == CardState.InSelectionAdd || DeckDisplay.instance.isRendering) {
            foreach(SpriteRenderer sr in cardParts) sr.sortingOrder = 10;
            cardParts[4].sortingOrder = 9; // set glow below the rest
            foreach(TextMeshPro tmp in textParts) tmp.sortingOrder = 12;
            tweenSequence.Append(tr.DOScale(1.25f * Vector3.one, .25f).SetId("zoomIn"));
            sm.PlaySound(sm.hoverSound);
        }
        
    }

    void OnMouseExit(){
        if(board.overlayActive && board.curPhase != Phase.Event) return;

        if(curState == CardState.InHand) {
            cardParts[5].sortingOrder = 1;
            foreach(SpriteRenderer sr in cardParts) sr.sortingLayerName = "UI Low";
            foreach(TextMeshPro tmp in textParts) tmp.sortingOrder = -1;
            DOTween.Pause("zoomIn");
            tweenSequence.Append(tr.DOScale(Vector3.one, .1f));
        } else if (curState == CardState.InSelectionRemove || curState == CardState.InSelectionAdd  || DeckDisplay.instance.isRendering) {
            foreach(SpriteRenderer sr in cardParts) sr.sortingOrder = 6;
            cardParts[4].sortingOrder = -1;
            foreach(TextMeshPro tmp in textParts) tmp.sortingOrder = 7;
            DOTween.Pause("zoomIn");
            tweenSequence.Append(tr.DOScale(Vector3.one * .8f, .1f));
        }
        
    }

    void OnMouseDown() {
        if(board.overlayActive && board.curPhase != Phase.Event) return;

        switch(board.curPhase){
            case Phase.Mulligan:
                // add card to the mulligan list if it isn't already in, and if it isn't locked, and if the mulligan limit isn't reached
                if(curState == CardState.InHand && !board.toMul.Contains(this.gameObject) && 
                   !board.lockedHand.Contains(this.gameObject)) {
                    if(board.toMul.Count == board.mulLimit) {
                        board.toMul[0].GetComponent<Card>().cardParts[5].enabled = false;
                        board.toMul[0] = this.gameObject;
                    } else {
                        board.toMul.Add(this.gameObject);
                    }
                    cardParts[5].enabled = true;
                    cardParts[5].sortingOrder = 15;
                    // FMOD Card Select Event
                    sm.PlaySound(sm.selectSound);
                }
                else if(board.toMul.Contains(this.gameObject) && !board.lockedHand.Contains(this.gameObject)) {
                    board.toMul.Remove(this.gameObject);
                    cardParts[5].enabled = false;
                    // FMOD Card Deselect Event
                    sm.PlaySound(sm.deselectSound);
                }
                break;
            case Phase.Play:
                if(curState == CardState.InHand) {
                    GetComponent<TrailRenderer>().enabled = true;
                    curState = CardState.InPlay;
                    prevParent = tr.parent;
                    tr.parent = null;
                    if(cardProps[0] == "Attack") {
                        foreach(GameObject enemy in board.enemies) {
                            enemy.transform.Find("TargetingFrame").GetComponent<SpriteRenderer>().enabled = true;
                        }
                    }
                }
                break;
            default:
                Debug.Log("reached unknown phase on click");
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collided) {
        if(curState != CardState.InPlay || collided == null || cardProps[0] == "Defend") return;
        if(collided.name == "EnemyColliderDefault") {
            SpriteRenderer targetingFrame = collided.transform.parent.Find("TargetingFrame").GetComponent<SpriteRenderer>();
            targetingFrame.sprite = targetingFrame.GetComponent<TargetingFrameRenderer>().frames[1];
        }      
    }

    void OnTriggerExit2D(Collider2D collided) {
        if(curState != CardState.InPlay || collided == null || cardProps[0] == "Defend") return;
        
        if(collided.name == "EnemyColliderDefault") {
            SpriteRenderer targetingFrame = collided.transform.parent.Find("TargetingFrame").GetComponent<SpriteRenderer>();
            targetingFrame.sprite = targetingFrame.GetComponent<TargetingFrameRenderer>().frames[0];
        }      
    }

    void OnMouseUpAsButton() {
        if(curState == CardState.InPlay) {
            if(board.playSequence.totalTime + this.cost > 15) {
                tr.parent = prevParent;
                prevParent = null;
                foreach(TextMeshPro tmp in textParts) tmp.sortingOrder = 7;
                curState = curState == CardState.InQueue ? CardState.InQueue : CardState.InHand;
                isSettled = false;
                return;
            }

            if(this.cardProps[0] == "Defend") {
                GetComponent<TrailRenderer>().enabled = false;
                float dist = Vector3.Distance(this.transform.position, this.prevParent.position);
                if(dist > 3) {
                    PlayerAction toInsert = new PlayerAction(this, board.player);
                    this.action = toInsert;
                    this.target = board.player;
                    toInsert.completeTime = board.playSequence.totalTime + toInsert.card.cost; // TODO: integrate this calculation as a method on Action?
                    board.playSequence.Add(toInsert);
                    OnEnqueue();
                    curState = CardState.InQueue;
                    // FMOD Card Play Confirmation Sound
                    sm.PlaySound(sm.confirmCardSound); 
                }
            } else {
                GetComponent<TrailRenderer>().enabled = false;
                foreach(GameObject enemy in board.enemies) {
                    SpriteRenderer targetingFrame = enemy.transform.Find("TargetingFrame").GetComponent<SpriteRenderer>();
                    targetingFrame.sprite = targetingFrame.GetComponent<TargetingFrameRenderer>().frames[0];
                    targetingFrame.enabled = false;
                }

                Collider2D[] colliders = Physics2D.OverlapPointAll(new Vector2(transform.position.x, transform.position.y));            
                foreach(Collider2D collider in colliders) {
                    if(collider.GetComponentInParent<SpriteRenderer>() == null) continue;

                    if(collider.GetComponentInParent<SpriteRenderer>().sortingLayerName == "Targets") {
                        PlayerAction toInsert = new PlayerAction(this, collider.gameObject);
                        this.action = toInsert;
                        this.target = collider.gameObject;
                        toInsert.completeTime = board.playSequence.totalTime + toInsert.card.cost; // TODO: integrate this calculation as a method on Action?
                        board.playSequence.Add(toInsert);
                        OnEnqueue();
                        curState = CardState.InQueue;
                        // FMOD Card Play Confirmation Sound
                        sm.PlaySound(sm.confirmCardSound);
                        break;
                    }
                }
            }

            // reanchor to old hand pos
            tr.parent = prevParent;
            prevParent = null;
            foreach(TextMeshPro tmp in textParts) tmp.sortingOrder = 7;
            curState = curState == CardState.InQueue ? CardState.InQueue : CardState.InHand;
            isSettled = false; // initiates tween back to hand pos
        } else if(curState == CardState.InSelectionRemove) {
            RemoveCardEvent removeEvent = Object.FindObjectOfType<RemoveCardEvent>();
            if(!removeEvent.toRemove.Contains(this.gameObject)) {
                removeEvent.toRemove.Add(this.gameObject);
                cardParts[4].enabled = true;
                cardParts[4].sortingLayerName = "Above Darkness";
                cardParts[4].sortingOrder = -1;
            } else {
                removeEvent.toRemove.Remove(this.gameObject);
                cardParts[4].enabled = false;
                cardParts[4].sortingLayerName = "UI Low";
            }

            if(removeEvent.toRemove.Count == 2) {
                removeEvent.callBaseResolve();
                for(int i = removeEvent.toRemove.Count - 1; i >= 0; i--) {
                    board.deck.Remove(removeEvent.toRemove[i]);
                    GameObject.Find("_DeckRenderer").GetComponent<DeckDisplay>().DeckOffScreen();
                    Destroy(removeEvent.toRemove[i]);
                }
            }
        } else if(curState == CardState.InSelectionAdd) {
            AddCardEvent addEvent = Object.FindObjectOfType<AddCardEvent>();
            if(!addEvent.toAdd.Contains(this.gameObject)) {
                addEvent.toAdd.Add(this.gameObject);
                cardParts[4].enabled = true;
                cardParts[4].sortingLayerName = "Above Darkness";
                cardParts[4].sortingOrder = -1;
            } else {
                addEvent.toAdd.Remove(this.gameObject);
                cardParts[4].enabled = false;
                cardParts[4].sortingLayerName = "UI Low";
            }

            if(addEvent.toAdd.Count == 2) {
                addEvent.callBaseResolve();
                GameObject.Find("_DeckRenderer").GetComponent<DeckDisplay>().DeckOffScreen();

                for(int i = addEvent.toAdd.Count - 1; i >= 0; i--) {
                    Debug.Log(i);
                    board.deck.Add(addEvent.toAdd[i]);
                    Card cardScript = addEvent.toAdd[i].GetComponent<Card>();
                    cardScript.textParts[0].text = cardName;
                    cardScript.textParts[1].text = desc;
                    cardScript.textParts[2].text = cost.ToString();
                    for (int j = 0; j < 3; j++ ) {
                        cardScript.textParts[j].sortingOrder = -1;
                        cardScript.textParts[j].sortingLayerID = SortingLayer.NameToID("UI High");
                    }
                    cardScript.isSettled = false;
                    cardScript.curState = CardState.InDeck;
                    cardScript.transform.parent = board.cardAnchors["Deck Anchor"];
                    board.addDeck.Remove(addEvent.toAdd[i]);
                    addEvent.toAdd.RemoveAt(i);
                }

                

                board.FisherYatesShuffle(board.deck);
            }
        }
    }
    
}
