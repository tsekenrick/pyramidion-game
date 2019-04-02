using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType { Attack, Defense, Status };

public class EnemyAction: Action
{
    public GameObject owner;
    public ActionType actionType;
    public int actionVal;
    public string statusType;
    public SoundManager sm;

    public EnemyAction(ActionType actionType, GameObject target, GameObject owner) : base(target) {
        this.actionType = actionType;
        if(this.actionType == ActionType.Attack || this.actionType == ActionType.Defense) {
            this.actionVal = Random.Range(5, 16);
        } // later an else block to account for ActionType.Status

        // assign resolution time for action at a random range, offset by the time carryover from last turn
        this.completeTime = Random.Range(2, 8) - Board.me.borrowedTime;
        this.owner = owner;
    }

    public void resolveAction() {
        Target target = this.target.GetComponent<Target>();
        switch(actionType) {
            case ActionType.Attack:
                // FMOD Enemy Attack Sound
                sm = SoundManager.me;
                sm.PlayEnemyAttackSound();

                int tmpBlock = target.block;
                target.block = Mathf.Max(target.block - actionVal, 0);
                target.health -= Mathf.Max(actionVal - tmpBlock, 0);
                break;

            case ActionType.Defense:
                // FMOD Enemy Defense Sound
                sm = SoundManager.me;
                sm.PlayEnemyDefendSound();

                target.block += actionVal;
                break;
        }
    }

}
