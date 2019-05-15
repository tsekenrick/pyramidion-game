using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TimelineTooltip : Tooltip {


    protected override void OnMouseEnter() {
        if(!transform.Find("TimelineGlow").GetComponent<SpriteRenderer>().enabled) return;
        base.OnMouseEnter();

        if(Board.instance.borrowedTime > 0) {
            tmp.text = $"Enemies are acting {Board.instance.borrowedTime} time units sooner than usual due to overplaying on the previous turn.";
        } else if(Board.instance.borrowedTime < 0) {
            tmp.text = $"Enemies are acting {Mathf.Abs(Board.instance.borrowedTime)} time units later than usual due to underplaying on the previous turn.";
        } else {
            tmp.text = $"Enemies are acting at their regular time because you played equal to their actions on the previous turn.";
        }
    }
    
}
