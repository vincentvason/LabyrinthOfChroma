using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyProperties
{
    // Movement
    public List<Vector3> waypoints;
    public float duration {get; set;}
    public int loop {get; set;} = 1;
    public bool destroyWhenOutside {get; set;} = true;
    public PathType pathType {get; set;} = PathType.Linear;
    public Ease ease {get; set;} = Ease.Linear;

    // Shooting
    public int bulletLines {get; set;} = 10;
    public float bulletSpeed {get; set;} = 5f;
    public float fireSeconds {get; set;} = 2f;
    public float startAngle {get; set;} = 90f;
    public float endAngle {get; set;} = 270f;
    public bool trackPlayer {get; set;} = false;

    // Core
    public int specialDamage {get; set;} = 3;
    public int hitPoints {get; set;}
    public bool isWild {get; set;} = true;
    public int orbTypeNeed {get; set;} 
    public bool isKeyEnemy {get; set;} = false;

    // Shield Container
    public int orbTypeListCount {get; set;} 

    public void waypointsAddPlayerPosition()
    {
        GameObject player = GameObject.Find("Player");
        waypoints.Add(player.transform.position);
    }
}
