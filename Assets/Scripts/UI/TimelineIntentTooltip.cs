using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineIntentTooltip : Tooltip {

    protected override void OnMouseEnter() {
        if(Board.instance.curPhase != Phase.Play) return;
        base.OnMouseEnter();
        EnemyIntentRenderer icon = GetComponent<EnemyIntentRenderer>();

        switch(icon.action.actionType) {
            case ActionType.Attack:
                tmp.text = $"Attacking for {icon.action.actionVal}";
                break;
            case ActionType.Defense:
                tmp.text = $"Defending for {icon.action.actionVal}";
                break;
            case ActionType.Summon:
                tmp.text = "Summoning a minion.";
                break;
        }
    }

    protected override void OnMouseExit() {
        if(Board.instance.curPhase != Phase.Play) return;
        base.OnMouseExit();
    }

}
