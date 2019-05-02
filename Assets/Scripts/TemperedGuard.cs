using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TemperedGuard : Card {
    
    public override void resolveAction() {
        base.resolveAction();
        Debug.Log("resolved in superclass");
    }

    public override void Awake() {
        base.Awake();
    }

    public override void Update() {
        base.Update();
    }
    
}
