using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModFightEvent : Event {
    
    protected override void Start() {
        base.Start();
    }

    void Update() {
        
    }

    protected override void ResolveEvent() {
        Board.instance.spawnEnemiesAtHealth = .8f;
        base.ResolveEvent();
    }
}
