using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedjedAdd : Medjed {

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public override IEnumerator Die() {
        GameObject.FindObjectOfType<Anubis>().addCount--;
        yield return StartCoroutine(base.Die());
    }
}
