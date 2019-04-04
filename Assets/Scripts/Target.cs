using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Target : MonoBehaviour
{
    public int health;
    public int block;

    public Transform healthBar;
    public Sprite[] combatStates;
    public SpriteRenderer[] srs;
    public SpriteRenderer[] blockOverlay;

    protected IEnumerator InjuryEffect() {
        yield return new WaitForSeconds(1f);
        srs[srs.Length -1].enabled = true;
        yield return new WaitForSeconds(.5f);
        srs[srs.Length - 1].enabled = false;
        prevHealth = health;
    }

}
