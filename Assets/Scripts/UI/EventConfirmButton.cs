using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventConfirmButton : MenuButton {

    public Event eventInstance;
    
    protected override void OnMouseUpAsButton() {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponentInChildren<TextMeshPro>().enabled = false;
        base.OnMouseUpAsButton();        
        eventInstance.ResolveConfirm();
        Destroy(gameObject);
    }
}


