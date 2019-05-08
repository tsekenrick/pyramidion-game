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
        end = -30f;
        start = 30f;
        this.transform.DOMoveX(end, firstLoopTime).OnComplete(MoveCloud);

    }

    public void MoveCloud() {
        this.transform.position = new Vector3(start, transform.position.y, transform.position.z);
        this.transform.DOMoveX(end, otherLoopsTime).OnComplete(MoveCloud);
    }

}
