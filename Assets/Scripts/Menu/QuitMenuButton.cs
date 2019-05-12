using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitMenuButton : MenuButton {
    
    protected override void OnMouseUpAsButton() {
        base.OnMouseUpAsButton();
        Application.Quit();
    }
}
