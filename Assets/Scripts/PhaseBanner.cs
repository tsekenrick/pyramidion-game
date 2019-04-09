using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class PhaseBanner : MonoBehaviour
{
    public bool isSettled;
    private Board board;
    public TextMeshPro phaseName;

    private IEnumerator MoveBanner() {
        this.transform.DOMoveX(0f, 1.25f).SetEase(Ease.OutElastic);
        yield return new WaitForSeconds(2f);
        this.transform.DOMoveX(20f, 1f).SetEase(Ease.OutCubic);
        isSettled = true;
    }

    void Start() {
        isSettled = false;
        board = Board.me;
        phaseName = GetComponentInChildren<TextMeshPro>();
        phaseName.text = "Mulligan Phase";
    }

    void Update() {
        if(isSettled) transform.position = new Vector3(-20, 0, 0);

        if(!isSettled) {
            StartCoroutine(MoveBanner());
        }
        
    }
}
