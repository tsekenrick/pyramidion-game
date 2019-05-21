using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class PhaseBanner : MonoBehaviour {

    private Board board;
    public TextMeshPro phaseName;
    public GameObject phaseBannerPrefab;
    private Sequence moveBanner;
    public bool canBanner;

    private IEnumerator MoveBanner() {
        this.transform.DOMoveX(0f, 1.25f).SetEase(Ease.OutElastic);
        yield return new WaitForSeconds(2f);
        moveBanner.Append(transform.DOMoveX(20f, .7f).SetEase(Ease.OutCubic));
        yield return new WaitForSeconds(1f);
        GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        this.transform.DOMoveX(-20f, .1f);
        GetComponent<SpriteRenderer>().sortingLayerName = "UI Mid";
        
    }
    
    public void DoBanner() {
        if (!canBanner) return;
        StartCoroutine(DisableBanner());

        // create new sequence and append desired movement behavior
        moveBanner = DOTween.Sequence();
        moveBanner.Append(transform.DOMoveX(0f, 1.15f).SetEase(Ease.OutExpo));
        moveBanner.AppendInterval(.75f);        
        moveBanner.Append(transform.DOMoveX(20f, .7f).SetEase(Ease.OutCubic));
        moveBanner.Append(transform.DOScale(Vector3.one * .01f, .01f));
        moveBanner.Append(transform.DOMoveX(-20f, .1f));
        moveBanner.Append(transform.DOScale(Vector3.one * .75f, .01f));

    }

    private IEnumerator DisableBanner() {
        canBanner = false;
        yield return new WaitForSeconds(2.75f);
        canBanner = true;
    }
  
    void Start() {
        DOTween.SetTweensCapacity(6250, 50);
        canBanner = true;
        DoBanner();
        board = Board.instance;
        phaseName = GetComponentInChildren<TextMeshPro>();
        phaseName.text = "Mulligan Phase";
    }

    void Update() {
        if(board.curPhase == Phase.Event) canBanner = false;
    }

}
