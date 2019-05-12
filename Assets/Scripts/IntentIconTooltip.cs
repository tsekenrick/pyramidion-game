using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntentIconTooltip : Tooltip {

    protected override void OnMouseEnter() {
        base.OnMouseEnter();
        MulliganIntentRenderer intentIconScript = GetComponent<MulliganIntentRenderer>();

        tooltipInstance.transform.localScale = new Vector3(.25f, .15f, .25f);
        tmp.transform.localScale = new Vector3(1f, 5/3f, 1f);
        switch(intentIconScript.iconIndex) {
            case 0:
                tmp.text = "This enemy intends to attack on this turn.";
                break;
            case 1:
                tmp.text = "This enemy intends to defend on this turn.";
                break;
            case 2:
                tmp.text = "This enemy intends to both attack and defend this turn.";
                break;
        }
    }

}
