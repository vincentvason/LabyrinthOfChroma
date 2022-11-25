using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCore : MonoBehaviour
{
    [Header("[Set] Player Settings")]
    [SerializeField] private GameObject player;

    [Header("[Set] Enemy Settings")]
    [SerializeField] private bool isWild;
    [SerializeField] private GameObject orbNeeded;
    [SerializeField] private int hitPoints;
    [SerializeField] private int scorePoints;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Update is called once per frame
    void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}
