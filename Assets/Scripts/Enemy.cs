using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Board board;

    private int health;
    private List<EnemyAction> prevActions;
    private List<EnemyAction> curActions;


    // Start is called before the first frame update
    void Start()
    {
        board = Board.me;
    }

    // Update is called once per frame
    void Update() {
        switch(board.curPhase) {
            case Phase.Mulligan:
                // not sure if these two lines work - check later if something is wrong
                prevActions = curActions;
                curActions.Clear(); 
                // roll for action type and display non-numerical intent
                if(curActions.Count == 0) {
                    for(int i = 0; i < Random.Range(0, 2); i++) { 
                        ActionType actionType = (ActionType)Random.Range(0, 2); // change to max 3 when we added statuses
                        // temporary cheeky one-liner - if it's an attack, target player, if not, target self
                        GameObject target = actionType == ActionType.Attack ? GameObject.Find("Player") : this.gameObject;
                        curActions.Add(new EnemyAction(actionType, target));
                        Debug.Log($"enemy will take action of type: {curActions[i].actionType}");
                    }

                }
                break;
            case Phase.Play:
                foreach(EnemyAction actionToAdd in curActions) {
                    if(!board.playSequence.Contains(actionToAdd)) {
                        int idx = board.playSequence.IndexOfCompleteTime(actionToAdd.completeTime);
                        board.playSequence.Insert(idx + 1, actionToAdd); // insert AFTER given index to give player priority in resolution
                    }
                }
                break;
        }
    }
}
