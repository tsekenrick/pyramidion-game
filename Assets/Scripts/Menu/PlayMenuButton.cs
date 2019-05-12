using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenuButton : MenuButton {
    [SerializeField]
    private int sceneToLoad;
    protected override void OnMouseUpAsButton() {
        base.OnMouseUpAsButton();
        SceneManager.LoadScene(sceneToLoad);
    }
}
