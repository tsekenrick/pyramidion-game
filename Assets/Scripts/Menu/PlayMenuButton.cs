using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenuButton : MenuButton {

    protected override void OnMouseUpAsButton() {
        base.OnMouseUpAsButton();
        SceneManager.LoadScene(1);
    }
}
