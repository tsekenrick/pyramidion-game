using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class EnemyIntentRenderer : MonoBehaviour
{
    public int completeTime;

    public Sprite[] intentIcons;
    private TextMeshPro textMesh;
    private SpriteRenderer sr;
    public EnemyAction action;
    public bool isSettled;
    // Start is called before the first frame update
    void Start() {
        isSettled = false;
        sr = this.GetComponent<SpriteRenderer>();
        textMesh = this.GetComponentInChildren<TextMeshPro>();
    }

    // Update is called once per frame
    void Update() {
        completeTime = action.completeTime;

        textMesh.text = action.actionVal.ToString();
        sr.sprite = intentIcons[(int)action.actionType];
        float xPos = Mathf.Max(0, action.completeTime * 1.14f);
        if(isSettled) this.transform.DOLocalMove(new Vector3(xPos, .98f, 0), .2f);

    }
}
