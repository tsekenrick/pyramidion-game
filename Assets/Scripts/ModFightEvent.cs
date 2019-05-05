using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModFightEvent : Event {
    
    protected override void Start() {
        base.Start();
    }

    void Update() {
        
    }

    protected override void resolveEvent() {
        Player player = Board.me.player.GetComponent<Player>();
        player.health += (int)(Player.MAX_HEALTH * .3f);
        base.resolveEvent();
    }
}
