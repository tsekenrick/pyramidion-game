using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CloudMover : MonoBehaviour {

    public float end;
    public float start;
    public float firstLoopTime;
    public float otherLoopsTime;

    void Start() {
        this.transform.DOMoveX(end, firstLoopTime).OnComplete(MoveCloud);
    }

    void Update() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .78f);
    }

    public void MoveCloud() {
        this.transform.position = new Vector3(start, transform.position.y, transform.position.z);
        this.transform.DOMoveX(end, otherLoopsTime).OnComplete(MoveCloud);
    }

}
