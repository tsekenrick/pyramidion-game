using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TargetingFrameRenderer : MonoBehaviour {
    public Sprite[] frames;

    void Start() {
        Vector3 initScale = transform.localScale;
        transform.DOScale(new Vector3(initScale.x + 0.03f, initScale.y + 0.03f, initScale.z + 0.03f), .75f).SetLoops(-1, LoopType.Yoyo);
    }
}
