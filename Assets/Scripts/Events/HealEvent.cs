using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealEvent : Event {

    protected override void Start() {
        base.Start();
    }

    void Update() {
        transform.Find("EventDesc").GetComponent<TextMeshPro>().text = $"Heal for 30% of max HP (currently {board.player.GetComponent<Player>().health}/100)";
    }

    protected override void ResolveEvent() {
        Player player = Board.instance.player.GetComponent<Player>();
        player.health += (int)(Player.MAX_HEALTH * .3f);
        base.ResolveEvent();
    }
}
