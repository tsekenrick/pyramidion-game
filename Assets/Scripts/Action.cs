using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public int completeTime;
    public GameObject target;
    public GameObject instance;

    public Action() {
        this.target = GameObject.Find("Player");
    }

    public Action(GameObject target) {
        this.target = target;
    }

}
