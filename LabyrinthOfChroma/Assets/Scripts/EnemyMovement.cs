using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyMovement : MonoBehaviour
{
    [Header("[Set] Player Tracking Location")]
    [HideInInspector] private Transform player;

    [Header("[Set] Enemy Speed Setting")]
    [SerializeField] private bool destroyWhenOutside;
    [SerializeField] private AnimationCurve speedX;
    [SerializeField] private AnimationCurve speedY;

    [Header("[Stat] Enemy Speed")]
    [SerializeField] private float time;
    [SerializeField] private float currentSpeedX;
    [SerializeField] private float currentSpeedY;
   
    public void SetMovement(AnimationCurve newSpeedX, AnimationCurve newSpeedY)
    {
        player = GameObject.Find("Player").transform;
        time = 0;
        speedX = newSpeedX;
        speedY = newSpeedY;
    }
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per step (0.02 seconds)
    void Update()
    {
        currentSpeedX = speedX.Evaluate(time);
        currentSpeedY = speedY.Evaluate(time);
        time += Time.deltaTime;
        transform.Translate(Time.deltaTime * currentSpeedX, Time.deltaTime * currentSpeedY, 0, Space.World);
    }
}
