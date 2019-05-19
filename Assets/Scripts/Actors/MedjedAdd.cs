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
        transform.Find("EnemyHealthBarBase").GetComponent<SpriteRenderer>().enabled = false;
        transform.Find("BasicShadow").GetComponent<SpriteRenderer>().enabled = false;
        transform.Find("MulliganIntent").GetComponent<SpriteRenderer>().enabled = false;
        yield return StartCoroutine(base.Die());
    }
}
