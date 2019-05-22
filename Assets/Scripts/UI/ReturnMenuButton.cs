using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnMenuButton : MenuButton {

    protected override void OnMouseUpAsButton() {
        base.OnMouseUpAsButton();
        transform.parent.GetComponent<InstructionsMenuButton>().isDisplaying = false;
    }
}
