using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TargetingFrameRenderer : MonoBehaviour {
    public Sprite[] frames;

    void Start() {
        transform.DOScale(Vector3.one * .73f, .75f).SetLoops(-1, LoopType.Yoyo);
    }
}
