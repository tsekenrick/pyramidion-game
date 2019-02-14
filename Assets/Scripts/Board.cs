using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    public static Board me;
    public GameObject player;
    public GameObject[] enemies;

    GameObject enemy;
    enum Phase {Mulligan, Play, Resolution};
    Phase curPhase;

    public GameObject cardPrefab;
    private List<GameObject> pool = new List<GameObject>();
    public Dictionary<string, Vector3> cardAnchors = new Dictionary<string, Vector3>();

    public List<Card> deck = new List<Card>();
    public List<Card> discard = new List<Card>();
    public List<Card> hand = new List<Card>();
    public int turn;


    void Awake(){
        me=this;
    }

    void Start(){
        Phase curPhase = Phase.Mulligan;

        // get anchor positions
        cardAnchors.Add("Deck Anchor", GameObject.Find("_DeckAnchor").transform.position);
        for(int i = 0; i < 5; i++){
            cardAnchors.Add($"Hand {i}", GameObject.Find($"Hand{i}").transform.position);
        }
        cardAnchors.Add("Discard Anchor", GameObject.Find("_DiscardAnchor").transform.position);

        for(int i = 0; i < 20; i++){
            GameObject curCard = Instantiate(cardPrefab, cardAnchors["Deck Anchor"], Quaternion.identity);
            pool.Add(curCard);
            curCard.SetActive(false);
        }

        // TODO: draw cards lol
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
