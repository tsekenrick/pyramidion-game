using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType { Attack, Defense, Status };

public class EnemyAction: Action
{
    public ActionType actionType;
    public int actionVal;

    public EnemyAction(ActionType actionType, GameObject target) : base(target) {
        this.actionType = actionType;
        if(this.actionType == ActionType.Attack || this.actionType == ActionType.Defense) {
            this.actionVal = Random.Range(5, 16);
        }
        this.completeTime = Random.Range(2, 11);
    }

    public void resolveAction() {
        // do things to resolve action
    }

}
