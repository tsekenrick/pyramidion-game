using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour {
    // FMOD variables
    private SoundManager sm;

    [SerializeField]
    protected Sprite[] buttonStates;

    void OnMouseEnter() {
        // FMOD Play button hover sound
        sm = SoundManager.me;
        sm.PlaySound(sm.menuButtonHoverSound);
        GetComponent<SpriteRenderer>().sprite = buttonStates[1];
    }

    void OnMouseExit() {
        GetComponent<SpriteRenderer>().sprite = buttonStates[0];
    }

    void OnMouseDown() {
        // FMOD Play button click sound
        sm = SoundManager.me;
        sm.PlaySound(sm.menuButtonClickSound);
        GetComponent<SpriteRenderer>().sprite = buttonStates[2];
    }

    protected virtual void OnMouseUpAsButton() {
        // FMOD Play button release sound
        sm = SoundManager.me;
        sm.PlaySound(sm.menuButtonReleaseSound);
        GetComponent<SpriteRenderer>().sprite = buttonStates[0];
    }

}
