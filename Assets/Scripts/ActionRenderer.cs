using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ActionRenderer : MonoBehaviour
{
    public Board board;
    public GameObject actionPrefab;
    public GameObject enemyActionPrefab;
    public GameObject[] enemies;
    private List<EnemyAction>[] enemyActions;
    public bool adjusted;
    
    private const float OFFSET = 1.2f;

    public IEnumerator AdjustForBorrowedTime(EnemyAction enemyAction) {

        yield return new WaitForSeconds(1f);
        enemyAction.instance.transform.DOShakePosition(.5f, .15f, 80, 45f);
        yield return new WaitForSeconds(.5f);
        enemyAction.instance.transform.DOLocalMove(new Vector3((enemyAction.completeTime) * 1.14f, .98f, 0), .2f);
        GameObject.Find("HourglassGlow").GetComponent<HourglassGlow>().isActive = false;
        GameObject.Find("HourglassGlow2").GetComponent<HourglassGlow>().isActive = false;
        GameObject.Find("TimelineGlow").GetComponent<HourglassGlow>().isActive = false;
    }

    private void Start() {
        board = Board.instance;
        adjusted = false;
    }

    private void Update() {
        enemies = board.enemies;
        enemyActions = new List<EnemyAction>[enemies.Length];
        for(int i = 0; i < enemyActions.Length; i++) enemyActions[i] = new List<EnemyAction>();
        
        // this should run once per turn
        if(board.curPhase != Phase.Event) {
            for(int i = 0; i < enemies.Length; i++) {
                if(enemyActions[i].Count == 0) {
                    enemyActions[i] = enemies[i].GetComponent<Enemy>().curActions;
                }
            }
        }
        
        // if in play phase - pull from enemy objects in scene and grab their curActions list
        if(board.curPhase == Phase.Play) {
            foreach(List<EnemyAction> actionList in enemyActions) {
                foreach(EnemyAction enemyAction in actionList) {
                    if(!enemyAction.instance) {
                        // do by enemyAction.owner
                        Enemy owner = enemyAction.owner.GetComponent<Enemy>();
                        enemyAction.instance = Instantiate(enemyActionPrefab, enemyAction.owner.transform.position, Quaternion.identity, this.transform);
                        enemyAction.instance.GetComponent<SpriteRenderer>().sprite = owner.intentIcons[(int)enemyAction.actionType];
                        enemyAction.instance.GetComponent<EnemyIntentRenderer>().action = enemyAction;
                        enemyAction.instance.transform.DOLocalMove(new Vector3((enemyAction.baseCompleteTime) * 1.14f, .98f, 0), .5f);
                        if(board.borrowedTime != 0) {
                            StartCoroutine(AdjustForBorrowedTime(enemyAction));
                        }
                    }
                }
            }
            
            if(!adjusted) StartCoroutine(GameObject.Find("BarOverlay").GetComponent<DarkProgressBar>().AdjustForBorrowedTime());   
        }

        foreach(Action entry in board.playSequence) {
            if(entry.GetType() == typeof(PlayerAction)) {
                PlayerAction action = entry as PlayerAction;
                if(!action.instance) {
                    action.instance = Instantiate(actionPrefab, action.target.transform.position, Quaternion.identity, this.transform);
                    action.instance.GetComponent<ActionInstance>().thisAction = action;
                    action.instance.GetComponent<SpriteRenderer>().size = new Vector2(action.card.cost * OFFSET, .45f);
                    action.instance.GetComponent<BoxCollider2D>().size = new Vector2(action.card.cost * OFFSET, .45f);
                    action.instance.GetComponent<BoxCollider2D>().offset = new Vector2(action.card.cost * .5f * OFFSET, 0f);
                    action.instance.GetComponentInChildren<TextMeshPro>().text = action.card.alias == null ? $"{action.card.cost}: {action.card.cardName}" : $"{action.card.cost}: {action.card.alias}";
                    action.instance.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(action.card.cost * OFFSET, .45f);
                    action.instance.transform.DOLocalMove(new Vector3((action.completeTime - action.card.cost) * 1.15f, 0, 0), .2f);

                    Transform playerIcon = action.instance.transform.Find("PlayerIcon");
                    playerIcon.localPosition = new Vector3(-0.23f + (action.card.cost * OFFSET), -.55f, playerIcon.position.z);
                } else {
                    action.instance.transform.DOLocalMove(new Vector3((action.completeTime - action.card.cost) * 1.15f, 0, 0), .2f);
                }
            }
            
        }
    }
}

