using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public static SoundManager me;

    // FMOD variables
    //located in player and creature action logic
    [FMODUnity.EventRef]
    public string enemyAttackSoundEvent;
    [FMODUnity.EventRef]
    public string enemyDefendSoundEvent;
    [FMODUnity.EventRef]
    public string playerAttackSoundEvent;
    [FMODUnity.EventRef]
    public string playerDefendSoundEvent;

    //located on board
    [FMODUnity.EventRef]
    public string dequeueCardSoundEvent;
    [FMODUnity.EventRef]
    public string lockSoundEvent;
    [FMODUnity.EventRef]
    public string toPlayPhaseSoundEvent;
    [FMODUnity.EventRef]
    public string toResolutionPhaseSoundEvent;
    [FMODUnity.EventRef]
    public string toMulliganPhaseSoundEvent;

    //located on card
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

    public FMOD.Studio.EventInstance dequeueCardSound;
    public FMOD.Studio.EventInstance lockSound;
    public FMOD.Studio.EventInstance toPlayPhaseSound;
    public FMOD.Studio.EventInstance toResolutionPhaseSound;
    public FMOD.Studio.EventInstance toMulliganPhaseSound;

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

        dequeueCardSound = FMODUnity.RuntimeManager.CreateInstance(confirmCardSoundEvent);
        lockSound = FMODUnity.RuntimeManager.CreateInstance(lockSoundEvent);
        toPlayPhaseSound = FMODUnity.RuntimeManager.CreateInstance(toPlayPhaseSoundEvent);
        toResolutionPhaseSound = FMODUnity.RuntimeManager.CreateInstance(toResolutionPhaseSoundEvent);
        toMulliganPhaseSound = FMODUnity.RuntimeManager.CreateInstance(toMulliganPhaseSoundEvent);

        drawSound = FMODUnity.RuntimeManager.CreateInstance(drawSoundEvent);
        hoverSound = FMODUnity.RuntimeManager.CreateInstance(hoverSoundEvent);
        discardSound = FMODUnity.RuntimeManager.CreateInstance(discardSoundEvent);
        shuffleSound = FMODUnity.RuntimeManager.CreateInstance(shuffleSoundEvent);
        selectSound = FMODUnity.RuntimeManager.CreateInstance(selectSoundEvent);
        deselectSound = FMODUnity.RuntimeManager.CreateInstance(deselectSoundEvent);
        confirmCardSound = FMODUnity.RuntimeManager.CreateInstance(confirmCardSoundEvent);

        confirmCardSound.setParameterValue("overplay", 0f);
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
    public void SetSoundParameter(FMOD.Studio.EventInstance soundToPlay, string parameter, float value)
    {
        soundToPlay.setParameterValue(parameter, value);
    }
}
