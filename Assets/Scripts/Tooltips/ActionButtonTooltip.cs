using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButtonTooltip : Tooltip {

    private Board board;
    private Vector3 startPos;
    protected void Start() {
        board = Board.instance;
        
    }

    protected override void OnMouseEnter() {
        if(board.curPhase != Phase.Play && board.curPhase != Phase.Mulligan) return;
        base.OnMouseEnter();
        
        tmp.sortingLayerID = SortingLayer.NameToID("UI High");
        sr.sortingLayerName = "UI High";
        tmp.sortingOrder = 5;
        sr.sortingOrder = 4;
        tmp.enableAutoSizing = false;
        tmp.fontSize = 5f;   

        switch(board.curPhase) {
            case Phase.Mulligan:

                tooltipInstance.transform.localScale = new Vector3(.35f, .25f, .5f);
                tmp.transform.localScale = new Vector3(1f, 1.4f, 1f);
                if(board.toMul.Count == 0) {
                    tmp.text = $"Stop redrawing and go to <b>Play Phase</b>, where you can play cards from your hand into the timeline.";
                } else {
                    tmp.text = $"Discard your selected cards and redraw from the draw pile.";;
                }
                
                break;
            case Phase.Play:
                tooltipInstance.GetComponent<TooltipBehavior>().yOffset = 1.2f;
                tmp.GetComponent<RectTransform>().sizeDelta = new Vector2(7.315f, 6.65f);
                tooltipInstance.transform.localScale = new Vector3(.5f, .6f, .5f);
                tmp.transform.localScale = new Vector3(1f, .5f/.6f, 1f);
                tmp.text = $"Stop playing cards and go to <b>Resolution Phase</b>, where actions in the timeline are executed one at a time.\n\nThe difference in time units between your last action and the enemy's last action is carried over to the next turn.";
                break;

        }
    }

    protected override void OnMouseExit() {
        if(board.curPhase != Phase.Play && board.curPhase != Phase.Mulligan) return;
        base.OnMouseExit();
    }
}
