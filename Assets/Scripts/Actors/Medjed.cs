using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medjed : Enemy {

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
        switch(board.curPhase) {
            case Phase.Mulligan:
                if(curActions.Count == 0) {
                    switch(board.round % 3) {
                        case 0:
                            curActions.Add(new EnemyAction(ActionType.Attack, board.player, this.gameObject, 7, 6));
                            break;
                        case 1:
                            curActions.Add(new EnemyAction(ActionType.Defense, this.gameObject, this.gameObject, 7, 4));
                            break;
                        case 2:
                            curActions.Add(new EnemyAction(ActionType.Defense, this.gameObject, this.gameObject, 4, 4));
                            curActions.Add(new EnemyAction(ActionType.Attack, board.player, this.gameObject, 16, 9));
                            break;
                    }
                }
                curActions.Sort((a, b) => a.completeTime.CompareTo(b.completeTime));
                break;
        }
    }
}
