using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TimelineTooltip : Tooltip {


    protected override void OnMouseEnter() {
        if(!transform.Find("TimelineGlow").GetComponent<SpriteRenderer>().enabled) return;
        base.OnMouseEnter();

        if(Board.me.borrowedTime > 0) {
            tmp.text = $"Enemies are attacking {Board.me.borrowedTime} time units sooner than usual due to overplaying on the previous turn.";
        } else if(Board.me.borrowedTime < 0) {
            tmp.text = $"Enemies are attacking {Mathf.Abs(Board.me.borrowedTime)} time units later than usual due to underplaying on the previous turn.";
        } else {
            tmp.text = $"Enemies are attacking at their regular time because you played equal to their actions on the previous turn.";
        }
    }
    
}
