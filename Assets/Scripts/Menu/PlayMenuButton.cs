using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenuButton : MenuButton {
    [SerializeField]
    private int sceneToLoad;
    protected override void OnMouseUpAsButton() {
        if(SceneManager.GetActiveScene().buildIndex == 0 && GameObject.Find("_DarknessOverlay").GetComponent<SpriteRenderer>().enabled) return;
        base.OnMouseUpAsButton();
        SceneManager.LoadScene(sceneToLoad);
    }

    protected override void OnMouseExit() {
        if(SceneManager.GetActiveScene().buildIndex == 0 && GameObject.Find("_DarknessOverlay").GetComponent<SpriteRenderer>().enabled) return;
        base.OnMouseExit();
    }

    protected override void OnMouseEnter() {
        if(SceneManager.GetActiveScene().buildIndex == 0 && GameObject.Find("_DarknessOverlay").GetComponent<SpriteRenderer>().enabled) return;
        base.OnMouseEnter();
    }

    protected override void OnMouseDown() {
        if(SceneManager.GetActiveScene().buildIndex == 0 && GameObject.Find("_DarknessOverlay").GetComponent<SpriteRenderer>().enabled) return;
        base.OnMouseDown();
    }
}
