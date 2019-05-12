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
        yield return new WaitForSeconds(.2f);
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
        Transform frame = this.action.owner.transform.Find("TargetingFrame");
        SpriteRenderer frameSr = frame.GetComponent<SpriteRenderer>();
        Vector3 initScale = frame.transform.localScale;
        frameSr.enabled = true;
    }

    void OnMouseExit() {
        Transform frame = this.action.owner.transform.Find("TargetingFrame");
        SpriteRenderer frameSr = frame.GetComponent<SpriteRenderer>();
        frameSr.enabled = false;
    }
}
