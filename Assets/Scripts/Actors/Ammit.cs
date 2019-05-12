using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammit : Enemy {
    private int turnCounter;

    protected override void Start() {
        base.Start();
        turnCounter = 0;
    }

    protected override void Update() {
        base.Update();
        switch(board.curPhase) {
            case Phase.Mulligan:
                if(curActions.Count == 0) {
                    switch(turnCounter) {
                        case 0:
                            float selector = Random.Range(0f, 1f);
                            if(selector <= .4f) {
                                turnCounter--;
                                curActions.Add(new EnemyAction(ActionType.Attack, board.player, this.gameObject, 14, 8));
                            } else if(selector <= .8f) {
                                turnCounter--;
                                curActions.Add(new EnemyAction(ActionType.Defense, this.gameObject, this.gameObject, 12, 6));
                                curActions.Add(new EnemyAction(ActionType.Attack, board.player, this.gameObject, 6, 8));
                            } else {
                                curActions.Add(new EnemyAction(ActionType.Defense, this.gameObject, this.gameObject, 25, 6));
                            }
                            break;
                        case 1:
                            curActions.Add(new EnemyAction(ActionType.Attack, board.player, this.gameObject, 25, 12));                
                            break;
                        case 2:
                            curActions.Add(new EnemyAction(ActionType.Defense, this.gameObject, this.gameObject, 2, 2));
                            curActions.Add(new EnemyAction(ActionType.Defense, this.gameObject, this.gameObject, 2, 4));
                            curActions.Add(new EnemyAction(ActionType.Defense, this.gameObject, this.gameObject, 2, 6));
                            curActions.Add(new EnemyAction(ActionType.Defense, this.gameObject, this.gameObject, 2, 8));
                            turnCounter = -1; // goes back to case 0 next turn
                            break;
                    }
                    turnCounter++;
                }
                curActions.Sort((a, b) => a.completeTime.CompareTo(b.completeTime));
                break;
        }
    }
}

