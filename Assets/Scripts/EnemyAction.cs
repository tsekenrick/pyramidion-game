using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType { Attack, Defense, Status };

public class EnemyAction: Action
{
    public ActionType actionType;
    public int actionVal;
    public string statusType;

    public EnemyAction(ActionType actionType, GameObject target) : base(target) {
        this.actionType = actionType;
        if(this.actionType == ActionType.Attack || this.actionType == ActionType.Defense) {
            this.actionVal = Random.Range(5, 16);
        } // later an else block to account for ActionType.Status
        this.completeTime = Random.Range(2, 8);
    }

    public void resolveAction() {
        Target target = this.target.GetComponent<Target>();
        switch(actionType) {
            case ActionType.Attack:
                int tmpBlock = target.block;
                target.block = Mathf.Max(target.block - actionVal, 0);
                target.health -= Mathf.Max(actionVal - tmpBlock, 0);
                break;

            case ActionType.Defense:
                target.block += actionVal;
                break;
        }
    }

}
