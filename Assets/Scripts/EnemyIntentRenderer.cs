using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class EnemyIntentRenderer : MonoBehaviour
{
    public int completeTime;

    private TextMeshPro textMesh;
    private SpriteRenderer sr;
    public EnemyAction action;
    private Sequence hoverSeq;

    public IEnumerator DestroyEnemyAction(GameObject actionInstance) {
        this.transform.DOLocalMove(new Vector3(0, .98f, 0), .2f);
        yield return new WaitForSeconds(.35f);
        Destroy(this.gameObject);
    }

    void Start() {
        sr = this.GetComponent<SpriteRenderer>();
        textMesh = this.GetComponentInChildren<TextMeshPro>();
    }

    void Update() {
        completeTime = action.completeTime;
        textMesh.text = action.actionVal.ToString();

    }

    void OnMouseEnter() {
        if(Board.me.curPhase == Phase.Resolution || Board.me.curPhase == Phase.Event) return;
        Transform frame = this.action.owner.transform.Find("TargetingFrame");
        SpriteRenderer frameSr = frame.GetComponent<SpriteRenderer>();
        Vector3 initScale = frame.transform.localScale;
        frameSr.enabled = true;
    }

    void OnMouseExit() {
        if(Board.me.curPhase == Phase.Resolution || Board.me.curPhase == Phase.Event) return;
        Transform frame = this.action.owner.transform.Find("TargetingFrame");
        SpriteRenderer frameSr = frame.GetComponent<SpriteRenderer>();
        frameSr.enabled = false;
    }
}
