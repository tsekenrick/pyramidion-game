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
                // roll for action type and display non-numerical intent
                if(curActions.Count == 0) {
                    for(int i = 0; i < Random.Range(2, 3); i++) { 
                        ActionType actionType = (ActionType)Random.Range(0, 2); // change to max 3 when we added statuses
                        // temporary cheeky one-liner - will not work for ActionType.Status
                        GameObject target = actionType == ActionType.Attack ? GameObject.Find("Player") : this.gameObject;
                        curActions.Add(new EnemyAction(actionType, target, this.gameObject));
                    }
                }
                curActions.Sort((a, b) => a.completeTime.CompareTo(b.completeTime));
                break;
        }
    }
}
