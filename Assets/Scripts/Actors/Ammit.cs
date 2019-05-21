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
                            curActions.Add(new EnemyAction(ActionType.Attack, board.player, this.gameObject, 16, 6));
                            turnCounter++;
                            break;
                        case 1:
                            curActions.Add(new EnemyAction(ActionType.Defense, this.gameObject, this.gameObject, 6, 2));
                            curActions.Add(new EnemyAction(ActionType.Attack, board.player, this.gameObject, 14, 8));
                            turnCounter++;
                            break;
                        case 2:
                            curActions.Add(new EnemyAction(ActionType.Defense, this.gameObject, this.gameObject, 25, 2));
                            turnCounter++;
                            break;
                        case 3:
                            curActions.Add(new EnemyAction(ActionType.Attack, board.player, this.gameObject, 25, 11));                
                            turnCounter = 0; // goes back to case 0 next turn
                            break;
                    }
                }
                curActions.Sort((a, b) => a.completeTime.CompareTo(b.completeTime));
                break;
        }
    }
}

