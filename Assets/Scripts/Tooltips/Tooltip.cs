using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Tooltip : MonoBehaviour {
    public GameObject tooltipPrefab;
    public GameObject tooltipInstance;
    protected SpriteRenderer sr;
    protected TextMeshPro tmp;

    protected virtual void OnMouseEnter() {
        if(Board.me.curPhase == Phase.Resolution || Board.me.curPhase == Phase.Event) return;
        StopAllCoroutines();
        tooltipInstance = Instantiate(tooltipPrefab, Vector3.zero, Quaternion.identity);
        sr = tooltipInstance.GetComponent<SpriteRenderer>();
        tmp = tooltipInstance.GetComponentInChildren<TextMeshPro>();
        
        Color original = sr.color;
        Color originalText = tmp.color;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
        tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 0);
        DOTween.To(() => sr.color, x => sr.color = x, original, .2f);
        DOTween.To(() => tmp.color, x => tmp.color = x, originalText, .2f);

    }
    
    protected virtual void OnMouseExit() {
        if(tooltipInstance == null || Board.me.curPhase == Phase.Resolution || Board.me.curPhase == Phase.Event) return;
        SpriteRenderer sr = tooltipInstance.GetComponent<SpriteRenderer>();
        TextMeshPro tmp = tooltipInstance.GetComponentInChildren<TextMeshPro>();
        
        StartCoroutine(FadeAndDestroy(sr, tmp));
    }

    protected IEnumerator FadeAndDestroy(SpriteRenderer sr, TextMeshPro tmp) {
        DOTween.To(() => sr.color, x => sr.color = x, new Color(sr.color.r, sr.color.g, sr.color.b, 0), .2f);
        DOTween.To(() => tmp.color, x => tmp.color = x, new Color(tmp.color.r, tmp.color.g, tmp.color.b, 0), .2f);
        yield return new WaitForSeconds(.25f);
        Destroy(tooltipInstance);
    }
}
