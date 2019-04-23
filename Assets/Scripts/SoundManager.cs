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

    [FMODUnity.EventRef]
    public string drawSoundEvent;
    [FMODUnity.EventRef]
    public string hoverSoundEvent;
    [FMODUnity.EventRef]
    public string discardSoundEvent;
    [FMODUnity.EventRef]
    public string shuffleSoundEvent;
    [FMODUnity.EventRef]
    public string selectSoundEvent;
    [FMODUnity.EventRef]
    public string deselectSoundEvent;
    [FMODUnity.EventRef]
    public string confirmCardSoundEvent;

    FMOD.Studio.EventInstance enemyAttackSound;
    FMOD.Studio.EventInstance enemyDefendSound;
    FMOD.Studio.EventInstance playerAttackSound;
    FMOD.Studio.EventInstance playerDefendSound;

    public FMOD.Studio.EventInstance drawSound;
    public FMOD.Studio.EventInstance hoverSound;
    public FMOD.Studio.EventInstance discardSound;
    public FMOD.Studio.EventInstance shuffleSound;
    public FMOD.Studio.EventInstance selectSound;
    public FMOD.Studio.EventInstance deselectSound;
    public FMOD.Studio.EventInstance confirmCardSound;

    private void Awake()
    {
        me = this;
        // FMOD object init
        enemyAttackSound = FMODUnity.RuntimeManager.CreateInstance(enemyAttackSoundEvent);
        enemyDefendSound = FMODUnity.RuntimeManager.CreateInstance(enemyDefendSoundEvent);
        playerAttackSound = FMODUnity.RuntimeManager.CreateInstance(playerAttackSoundEvent);
        playerDefendSound = FMODUnity.RuntimeManager.CreateInstance(playerDefendSoundEvent);

        drawSound = FMODUnity.RuntimeManager.CreateInstance(drawSoundEvent);
        hoverSound = FMODUnity.RuntimeManager.CreateInstance(hoverSoundEvent);
        discardSound = FMODUnity.RuntimeManager.CreateInstance(discardSoundEvent);
        shuffleSound = FMODUnity.RuntimeManager.CreateInstance(shuffleSoundEvent);
        selectSound = FMODUnity.RuntimeManager.CreateInstance(selectSoundEvent);
        deselectSound = FMODUnity.RuntimeManager.CreateInstance(deselectSoundEvent);
        confirmCardSound = FMODUnity.RuntimeManager.CreateInstance(confirmCardSoundEvent);
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
    public void PlaySound(FMOD.Studio.EventInstance soundToPlay)
    {
        soundToPlay.start();
    }
}
