using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusAttack : Card {

    public override void resolveAction() {
        base.resolveAction();
        Debug.Log("resolved in superclass");
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }
}
