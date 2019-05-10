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
    private bool stopAllTweens = false;

    public IEnumerator DestroyEnemyAction(GameObject actionInstance) {
        stopAllTweens = true;
        this.transform.DOLocalMove(new Vector3(0, .98f, 0), .2f);
        yield return new WaitForSeconds(.5f);
        Destroy(this.gameObject);
    }

    void Start() {
        sr = this.GetComponent<SpriteRenderer>();
        textMesh = this.GetComponentInChildren<TextMeshPro>();
    }

    // Update is called once per frame
    void Update() {
        completeTime = action.completeTime;
        textMesh.text = action.actionVal.ToString();
        float xPos = Mathf.Max(0, action.completeTime * 1.14f);
        if(!stopAllTweens && (Board.me.curPhase == Phase.Resolution || Board.me.curPhase == Phase.Mulligan)) this.transform.DOLocalMove(new Vector3(xPos, .98f, 0), .2f);

    }

    void OnMouseEnter() {
        this.action.owner.transform.Find("TargetingFrame").GetComponent<SpriteRenderer>().enabled = true;
    }

    void OnMouseExit() {
        this.action.owner.transform.Find("TargetingFrame").GetComponent<SpriteRenderer>().enabled = false;
    }
}
