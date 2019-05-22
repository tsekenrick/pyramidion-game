using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventConfirmButton : MenuButton {

    public Event eventInstance;
    
    protected override void OnMouseUpAsButton() {
        base.OnMouseUpAsButton();        
        eventInstance.ResolveConfirm();
    }
}


