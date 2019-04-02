using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public static SoundManager me;

    // FMOD variables
    [FMODUnity.EventRef]
    public string enemyAttackSoundEvent;
    [FMODUnity.EventRef]
    public string enemyDefendSoundEvent;
    [FMODUnity.EventRef]
    public string playerAttackSoundEvent;
    [FMODUnity.EventRef]
    public string playerDefendSoundEvent;

    FMOD.Studio.EventInstance enemyAttackSound;
    FMOD.Studio.EventInstance enemyDefendSound;
    FMOD.Studio.EventInstance playerAttackSound;
    FMOD.Studio.EventInstance playerDefendSound;

    private void Awake()
    {
        me = this;
        // FMOD object init
        enemyAttackSound = FMODUnity.RuntimeManager.CreateInstance(enemyAttackSoundEvent);
        enemyDefendSound = FMODUnity.RuntimeManager.CreateInstance(enemyDefendSoundEvent);
        playerAttackSound = FMODUnity.RuntimeManager.CreateInstance(playerAttackSoundEvent);
        playerDefendSound = FMODUnity.RuntimeManager.CreateInstance(playerDefendSoundEvent);
    }

    public void PlayEnemyAttackSound()
    {
        enemyAttackSound.start();
    }
    public void PlayEnemyDefendSound()
    {
        enemyDefendSound.start();
    }
    public void PlayPlayerAttackSound()
    {
        playerAttackSound.start();
    }
    public void PlayPlayerDefendSound()
    {
        playerDefendSound.start();
    }
}
