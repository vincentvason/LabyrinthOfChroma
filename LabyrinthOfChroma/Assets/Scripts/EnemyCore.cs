using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCore : MonoBehaviour
{
    [Header("[Set] Player Settings")]
    [SerializeField] private PlayerStats player;
    [SerializeField] private GameObject orbSpecial;

    [Header("[Set] Enemy Settings")]
    [SerializeField] private bool isWild = true;
    [SerializeField] private int orbTypeNeed;
    [SerializeField] private int specialDamage;
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
        if(collision.gameObject.tag == "Orb_Player")
        {
            if(collision.gameObject.name.Contains(orbSpecial.name))
            {
                hitPoints = hitPoints - specialDamage;
            }
            else if(isWild == true)
            {
                hitPoints = hitPoints - 1;
            }
            else if(collision.gameObject.GetComponent<OrbShooting>().orbTypeNumber == orbTypeNeed)
            {
                hitPoints = hitPoints - 1;
            }
            else
            {
                //Repel
            }
            if(hitPoints <= 0)
            {
                player.ScoreAdd(scorePoints);
                transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
