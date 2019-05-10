using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public enum ActionType { Attack, Defense, Status };

public class EnemyAction: Action
{
    public GameObject owner;
    public ActionType actionType;
    public int actionVal;
    public int baseCompleteTime; // complete time without borrowed time offset - used for animation
    public string statusType;
    public SoundManager sm;

    public EnemyAction(ActionType actionType, GameObject target, GameObject owner) : base(target) {
        this.actionType = actionType;
        if(this.actionType == ActionType.Attack || this.actionType == ActionType.Defense) {
            this.actionVal = Random.Range(5, 16);
        } // later an else block to account for ActionType.Status

        // assign resolution time for action at a random range, offset by the time carryover from last turn
        // also makes sure that there aren't multiple actions at the same time
        int choice = Random.Range(2, 8);
        List<int> existingTimes = new List<int>();
        if(owner.GetComponent<Enemy>().curActions.Count > 0) {
            foreach(Action action in owner.GetComponent<Enemy>().curActions) existingTimes.Add(action.completeTime);
            while (existingTimes.Contains(choice)) choice = Random.Range(2, 8); 
        }
        this.baseCompleteTime = choice;
        this.completeTime = Mathf.Min(15, (Mathf.Max(0, baseCompleteTime - Board.me.borrowedTime)));
        this.owner = owner;
    }

    public void resolveAction() {
        Target target = this.target.GetComponent<Target>();
        switch(actionType) {
            case ActionType.Attack:
                // FMOD Enemy Attack Sound
                sm = SoundManager.me;
                sm.PlayEnemyAttackSound();

                int tmpBlock = target.block;
                target.block = Mathf.Max(target.block - actionVal, 0);
                target.transform.Find("DamageText").GetComponent<TextMeshPro>().text = $"{Mathf.Max(actionVal - tmpBlock, 0)}";
                target.GetComponentInChildren<DamageText>().FadeText();
                target.health -= Mathf.Max(actionVal - tmpBlock, 0);
                if(Mathf.Max(actionVal - tmpBlock, 0) > 0) {
                    target.transform.Find("TakingDamagePS").GetComponent<ParticleSystem>().Play();
                    Camera.main.transform.DOShakePosition(.5f);
                } else {
                    target.transform.Find("DamagedShieldPS").GetComponent<ParticleSystem>().Play();
                    Camera.main.transform.DOShakePosition(.5f, .5f);
                }
                
                break;

            case ActionType.Defense:
                // FMOD Enemy Defense Sound
                sm = SoundManager.me;
                sm.PlayEnemyDefendSound();

                target.block += actionVal;
                target.transform.Find("ShieldPS").GetComponent<ParticleSystem>().Play();
                
                // Sequence animShield = DOTween.Sequence();
                // animShield.Append(target.transform.Find("HealthBarBase").Find("BlockIcon").DOScale(2f, .25f));
                // animShield.Append(target.transform.Find("HealthBarBase").Find("BlockIcon").DOScale(1f, .25f));
                break;
        }
        this.instance.transform.DOLocalMove(new Vector3(0, .98f, 0), .2f);
    }

}
