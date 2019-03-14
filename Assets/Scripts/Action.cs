using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    public Card card;
    public int completeTime;
    public GameObject target;
    public GameObject instance;

    public Action(Card card, GameObject target) {
        this.card = card;
        this.target = target;
    }

}
