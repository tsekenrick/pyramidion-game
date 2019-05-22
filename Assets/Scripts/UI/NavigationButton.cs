using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationButton : MenuButton {

    public int incrementBy;
    protected override void OnMouseUpAsButton() {
        base.OnMouseUpAsButton();
        StartCoroutine(transform.parent.GetComponent<InstructionsMenuButton>().SwitchSlides(incrementBy));
    }
}
