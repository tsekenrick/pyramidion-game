using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageText : MonoBehaviour {

    private Color initColor;
    void Start() {
        initColor = this.GetComponent<TextMeshPro>().color;
    }

    public void FadeText() {
        TextMeshPro tmp = this.GetComponent<TextMeshPro>();
        tmp.color = initColor;
        DOTween.To(()=> tmp.color, x=> tmp.color = x, new Color32(180, 0, 0, 0), 1f);
    }
}
