using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSoundInstantiation : MonoBehaviour
{
    // FMOD variables
    public GameObject soundManagerPrefab;

    private SoundManager sm = SoundManager.me;
    private void Awake()
    {
        // Spawn FMOD sound manager if it doesn't exist.
        if (GameObject.FindGameObjectWithTag("SoundManager")) { }
        else
            Instantiate(soundManagerPrefab);
    }

    private void Start()
    {
        // FMOD set parameter for ambience game state to 0
        sm = SoundManager.me;
        sm.SetSoundParameter(sm.ambienceSound, "InGamplayScene", 0f);
    }
}
