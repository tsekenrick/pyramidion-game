using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public GameObject[] enemyList;
    public GameObject boss;
    
    public void SpawnRandomEnemy() {
        Instantiate(enemyList[Random.Range(0, enemyList.Length)], Vector3.zero, Quaternion.identity, this.transform);
    }

    public void SpawnEnemy(int enemyType, int enemyPosition) {
        GameObject enemy = Instantiate(enemyList[enemyType], this.transform, false);
        enemy.GetComponent<Enemy>().health = (int)(enemy.GetComponent<Enemy>().maxHealth * Board.instance.spawnEnemiesAtHealth);
        enemy.transform.localPosition = new Vector3(enemyPosition * -5.5f, 0, 9.3f);
    }
}
