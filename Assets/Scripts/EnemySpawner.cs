using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public GameObject[] enemyList;

    public void SpawnRandomEnemy() {
        Instantiate(enemyList[Random.Range(0, enemyList.Length)], Vector3.zero, Quaternion.identity, this.transform);
    }
}
