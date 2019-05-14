using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCardEvent : Event {
    
    protected override void Start() {
        base.Start();
    }

    void Update() {
        
    }

    protected override void ResolveEvent() {
        Player player = Board.instance.player.GetComponent<Player>();
        player.health += (int)(Player.MAX_HEALTH * .3f);
        base.ResolveEvent();
    }
}
