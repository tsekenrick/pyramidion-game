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
    [FMODUnity.EventRef]
    public string summonSoundEvent;
    [FMODUnity.EventRef]
    public string enrageSoundEvent;
    [FMODUnity.EventRef]
    public string playerBuffSoundEvent;

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
    [FMODUnity.EventRef]
    public string overplayPunishmentSoundEvent;

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

    //located on event
    [FMODUnity.EventRef]
    public string hoverEventButtonSoundEvent;
    [FMODUnity.EventRef]
    public string clickEventButtonSoundEvent;

    //located on cardpile button
    [FMODUnity.EventRef]
    public string pileHoverSoundEvent;
    [FMODUnity.EventRef]
    public string pileSelectSoundEvent;
    [FMODUnity.EventRef]
    public string pileDeselectSoundEvent;

    //located on MenuButton
    [FMODUnity.EventRef]
    public string menuButtonHoverSoundEvent;
    [FMODUnity.EventRef]
    public string menuButtonClickSoundEvent;
    [FMODUnity.EventRef]
    public string menuButtonReleaseSoundEvent;

    //located on ActionButton
    [FMODUnity.EventRef]
    public string actionButtonDownSoundEvent;
    [FMODUnity.EventRef]
    public string actionButtonUpSoundEvent;
    [FMODUnity.EventRef]
    public string actionButtonHoverSoundEvent;

    //located on sound manager
    [FMODUnity.EventRef]
    public string ambienceSoundEvent;

    // FMOD Snapshots
    [FMODUnity.EventRef]
    public string battleSnapshotEvent;
    [FMODUnity.EventRef]
    public string punishmentSnapshotEvent;


    //located in player and creature action logic
    FMOD.Studio.EventInstance enemyAttackSound;
    FMOD.Studio.EventInstance enemyDefendSound;
    FMOD.Studio.EventInstance playerAttackSound;
    FMOD.Studio.EventInstance playerDefendSound;
    FMOD.Studio.EventInstance summonSound;
    FMOD.Studio.EventInstance enrageSound;
    FMOD.Studio.EventInstance playerBuffSound;

    //located on board
    public FMOD.Studio.EventInstance dequeueCardSound;
    public FMOD.Studio.EventInstance lockSound;
    public FMOD.Studio.EventInstance toPlayPhaseSound;
    public FMOD.Studio.EventInstance toResolutionPhaseSound;
    public FMOD.Studio.EventInstance toMulliganPhaseSound;
    public FMOD.Studio.EventInstance overplayPunishmentSound;

    //located on card
    public FMOD.Studio.EventInstance drawSound;
    public FMOD.Studio.EventInstance hoverSound;
    public FMOD.Studio.EventInstance discardSound;
    public FMOD.Studio.EventInstance shuffleSound;
    public FMOD.Studio.EventInstance selectSound;
    public FMOD.Studio.EventInstance deselectSound;
    public FMOD.Studio.EventInstance confirmCardSound;

    //located on event
    public FMOD.Studio.EventInstance hoverEventButtonSound;
    public FMOD.Studio.EventInstance clickEventButtonSound;

    //located on cardpile button
    public FMOD.Studio.EventInstance pileHoverSound;
    public FMOD.Studio.EventInstance pileSelectSound;
    public FMOD.Studio.EventInstance pileDeselectSound;

    //located on MenuButton
    public FMOD.Studio.EventInstance menuButtonHoverSound;
    public FMOD.Studio.EventInstance menuButtonClickSound;
    public FMOD.Studio.EventInstance menuButtonReleaseSound;

    //located on ActionButton
    public FMOD.Studio.EventInstance actionButtonDownSound;
    public FMOD.Studio.EventInstance actionButtonUpSound;
    public FMOD.Studio.EventInstance actionButtonHoverSound;

    //located on sound manager
    public FMOD.Studio.EventInstance ambienceSound;

    //snapshots
    public FMOD.Studio.EventInstance battleSnapshot;
    public FMOD.Studio.EventInstance punishmentSnapshot;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        me = this;
        // FMOD object init
        //located in player and creature action logic
        enemyAttackSound = FMODUnity.RuntimeManager.CreateInstance(enemyAttackSoundEvent);
        enemyDefendSound = FMODUnity.RuntimeManager.CreateInstance(enemyDefendSoundEvent);
        playerAttackSound = FMODUnity.RuntimeManager.CreateInstance(playerAttackSoundEvent);
        playerDefendSound = FMODUnity.RuntimeManager.CreateInstance(playerDefendSoundEvent);
        summonSound = FMODUnity.RuntimeManager.CreateInstance(summonSoundEvent);
        enrageSound = FMODUnity.RuntimeManager.CreateInstance(enrageSoundEvent);
        playerBuffSound = FMODUnity.RuntimeManager.CreateInstance(playerBuffSoundEvent);

       //located on board
        dequeueCardSound = FMODUnity.RuntimeManager.CreateInstance(dequeueCardSoundEvent);
        lockSound = FMODUnity.RuntimeManager.CreateInstance(lockSoundEvent);
        toPlayPhaseSound = FMODUnity.RuntimeManager.CreateInstance(toPlayPhaseSoundEvent);
        toResolutionPhaseSound = FMODUnity.RuntimeManager.CreateInstance(toResolutionPhaseSoundEvent);
        toMulliganPhaseSound = FMODUnity.RuntimeManager.CreateInstance(toMulliganPhaseSoundEvent);
        overplayPunishmentSound = FMODUnity.RuntimeManager.CreateInstance(overplayPunishmentSoundEvent);

        //located on card
        drawSound = FMODUnity.RuntimeManager.CreateInstance(drawSoundEvent);
        hoverSound = FMODUnity.RuntimeManager.CreateInstance(hoverSoundEvent);
        discardSound = FMODUnity.RuntimeManager.CreateInstance(discardSoundEvent);
        shuffleSound = FMODUnity.RuntimeManager.CreateInstance(shuffleSoundEvent);
        selectSound = FMODUnity.RuntimeManager.CreateInstance(selectSoundEvent);
        deselectSound = FMODUnity.RuntimeManager.CreateInstance(deselectSoundEvent);
        confirmCardSound = FMODUnity.RuntimeManager.CreateInstance(confirmCardSoundEvent);

        //located on event
        hoverEventButtonSound = FMODUnity.RuntimeManager.CreateInstance(hoverEventButtonSoundEvent);
        clickEventButtonSound = FMODUnity.RuntimeManager.CreateInstance(clickEventButtonSoundEvent);

        //located on cardpile button
        pileHoverSound = FMODUnity.RuntimeManager.CreateInstance(pileHoverSoundEvent);
        pileSelectSound = FMODUnity.RuntimeManager.CreateInstance(pileSelectSoundEvent);
        pileDeselectSound = FMODUnity.RuntimeManager.CreateInstance(pileDeselectSoundEvent);

        //located on MenuButton
        menuButtonHoverSound = FMODUnity.RuntimeManager.CreateInstance(menuButtonHoverSoundEvent);
        menuButtonClickSound = FMODUnity.RuntimeManager.CreateInstance(menuButtonClickSoundEvent);
        menuButtonReleaseSound = FMODUnity.RuntimeManager.CreateInstance(menuButtonReleaseSoundEvent);

        //located on ActionButton
        actionButtonDownSound = FMODUnity.RuntimeManager.CreateInstance(actionButtonDownSoundEvent);
        actionButtonUpSound = FMODUnity.RuntimeManager.CreateInstance(actionButtonUpSoundEvent);
        actionButtonHoverSound = FMODUnity.RuntimeManager.CreateInstance(actionButtonHoverSoundEvent);

    //located on sound manager
    ambienceSound = FMODUnity.RuntimeManager.CreateInstance(ambienceSoundEvent);

        //snapshots
        battleSnapshot = FMODUnity.RuntimeManager.CreateInstance(battleSnapshotEvent);
        punishmentSnapshot = FMODUnity.RuntimeManager.CreateInstance(punishmentSnapshotEvent);

        //parameter init
        confirmCardSound.setParameterValue("overplay", 0f);
        ambienceSound.setParameterValue("DayNight", 0f);
    }

    private void Start()
    {
        ambienceSound.start();
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
    public void PlaySummonSound()
    {
        summonSound.start();
    }
    public void PlayEnrageSound()
    {
        enrageSound.start();
    }
    public void PlayPlayerBuffSound()
    {
        playerBuffSound.start();
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
