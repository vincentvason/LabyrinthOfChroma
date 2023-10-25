using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] public GameObject basicEnemy;
    [SerializeField] public GameObject lineEnemy;
    [SerializeField] public GameObject oneShieldEnemy;
    [SerializeField] public GameObject twoShieldEnemy;
    [SerializeField] public GameObject threeShieldEnemy;

    public void SpawnTutorialWave0()
    {
        EnemyProperties enemy;
        enemy = new EnemyProperties();
        enemy.waypoints.Add(new Vector3(5.5f,2f,0f));
        enemy.waypoints.Add(new Vector3(1f,2f,0f));
        enemy.duration = 4.5f;
        enemy.ease = Ease.OutCirc;
        enemy.bulletLines = 1;
        enemy.bulletSpeed = 5f;
        enemy.startAngle = 180f;
        enemy.endAngle = 180f;
        SpawnBasicEnemy(enemy);

        enemy = new EnemyProperties();
        enemy.waypoints.Add(new Vector3(5.5f,1f,0f));
        enemy.waypoints.Add(new Vector3(2f,1f,0f));
        enemy.duration = 4.5f;
        enemy.ease = Ease.OutCirc;
        enemy.bulletLines = 1;
        enemy.bulletSpeed = 5f;
        enemy.startAngle = 180f;
        enemy.endAngle = 180f;
        SpawnBasicEnemy(enemy);

        enemy = new EnemyProperties();
        enemy.waypoints.Add(new Vector3(5.5f,-1f,0f));
        enemy.waypoints.Add(new Vector3(2f,-1f,0f));
        enemy.duration = 4.5f;
        enemy.ease = Ease.OutCirc;
        enemy.bulletLines = 1;
        enemy.bulletSpeed = 5f;
        enemy.startAngle = 180f;
        enemy.endAngle = 180f;
        SpawnBasicEnemy(enemy);

        enemy = new EnemyProperties();
        enemy.waypoints.Add(new Vector3(5.5f,-2f,0f));
        enemy.waypoints.Add(new Vector3(1f,-2f,0f));
        enemy.duration = 4.5f;
        enemy.ease = Ease.OutCirc;
        enemy.bulletLines = 1;
        enemy.bulletSpeed = 5f;
        enemy.startAngle = 180f;
        enemy.endAngle = 180f;
        SpawnBasicEnemy(enemy);
    }

    void SpawnBasicEnemy(EnemyProperties enemyProperties)
    {
        GameObject enemy = Instantiate(basicEnemy);
        enemy.GetComponent<EnemyMovement2>().SetMovement(enemyProperties);
        enemy.GetComponentInChildren<EnemyShooting>().SetShooting(enemyProperties);
        enemy.GetComponentInChildren<EnemyCore>().SetCore(enemyProperties);
        enemy.GetComponentInChildren<EnemyShieldContainer>().SetShieldContainer(enemyProperties);
    }
}
