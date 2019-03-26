using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ActionRenderer : MonoBehaviour
{
    public Board board;
    public GameObject actionPrefab;
    private const float OFFSET = 1.2f;
    // Start is called before the first frame update
    void Start() {
        board = Board.me;
    }

    // Update is called once per frame
    void Update() {
        foreach(Action entry in board.playSequence) {
            if(entry.GetType() == typeof(PlayerAction)) {
                PlayerAction action = entry as PlayerAction;
                if(!action.instance) {
                    action.instance = Instantiate(actionPrefab, action.target.transform.position, Quaternion.identity, this.transform);
                    action.instance.GetComponent<SpriteRenderer>().size = new Vector2(action.card.cost * OFFSET, .45f);
                    action.instance.GetComponentInChildren<TextMeshPro>().text = $"{action.card.cost}: {action.card.cardName}";
                    action.instance.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(action.card.cost * OFFSET, .45f);
                    action.instance.transform.DOLocalMove(new Vector3((action.completeTime - action.card.cost) * 1.15f, 0, 0), .2f);

                    // TODO: Add dequeueing functionality
                } else {
                    action.instance.transform.DOLocalMove(new Vector3((action.completeTime - action.card.cost) * 1.15f, 0, 0), .2f);
                }
            }
            
        }
    }
}

