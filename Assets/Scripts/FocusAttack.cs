using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FocusAttack : Card {

    public override void resolveAction() {
        base.resolveAction();
        Debug.Log("resolved in superclass");
    }

    // Start is called before the first frame update
    public override void Awake() {
        base.Awake();
    }

    // Update is called once per frame
    public override void Update() {
        base.Update();
    }
}
