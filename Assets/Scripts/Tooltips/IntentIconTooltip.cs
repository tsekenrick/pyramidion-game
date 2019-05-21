using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntentIconTooltip : Tooltip {

    protected void Update() {
        if(Board.instance.curPhase != Phase.Mulligan) {
            base.OnMouseExit();
        }
    }

    protected override void OnMouseEnter() {
        if(Board.instance.curPhase != Phase.Mulligan) return;
        base.OnMouseEnter();
        MulliganIntentRenderer intentIconScript = transform.parent.GetComponentInChildren<MulliganIntentRenderer>();

        tooltipInstance.transform.localScale = new Vector3(.25f, .15f, .25f);
        tmp.transform.localScale = new Vector3(1f, 5/3f, 1f);
        switch(intentIconScript.iconIndex) {
            case 0:
                tmp.text = "The enemy intends to attack on this turn.";
                break;
            case 1:
                tmp.text = "The enemy intends to defend on this turn.";
                break;
            case 2:
                tmp.text = "The enemy intends to both attack and defend this turn.";
                break;
            case 3:
                tmp.text = "The enemy intends to both attack and summon a minion next turn.";
                tooltipInstance.transform.localScale = new Vector3(.25f, .2f, .25f);
                tmp.transform.localScale = new Vector3(1f, 1.25f, 1f);
                break;
        }
    }

    protected override void OnMouseExit() {
        if(Board.instance.curPhase != Phase.Mulligan) return;
        base.OnMouseExit();
    }


}
