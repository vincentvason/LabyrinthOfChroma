using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCore : MonoBehaviour
{
    [Header("[Set] Player Settings")]
    [HideInInspector] private GameObject player;
    [HideInInspector] private GameObject enemyScene;
    [HideInInspector] private GameObject sound;
    [SerializeField] private EnemyShieldContainer shield;
    [SerializeField] private GameObject orbSpecial;

    [Header("[Set] Enemy Settings")]
    [SerializeField] private bool isWild = true;
    [SerializeField] private bool isKeyEnemy = false;
    [SerializeField] private int orbTypeNeed;
    [SerializeField] private int specialDamage = 3;
    [SerializeField] private int hitPoints;
    [SerializeField] private bool diedIfZeroOrb;
    [SerializeField] private float maxScore = 10000f;
    [SerializeField] private float minScore = 5000f;
    [SerializeField] private float bonusDuration = 5f;
    
    
    [HideInInspector] private float timeAlive;
    [HideInInspector] private int scorePoints;

    [SerializeField] private AudioSource hitSound;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Game System");
        sound = GameObject.Find("Sound System");
    }

    // Update is called once per frame
    void Update()
    {
        timeAlive = timeAlive + Time.deltaTime;

        float bonusPerSecond = (maxScore - minScore) / bonusDuration;
        scorePoints = (int)(Mathf.Clamp(maxScore - (timeAlive * bonusPerSecond), minScore, maxScore));

        if(diedIfZeroOrb == true && shield.GetOrbCount() <= 0)
        {
            hitPoints = 0;
            DamageEnemy();
        }
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
        }
        DamageEnemy();
    }

    private void DamageEnemy()
    {
        
        if(hitPoints <= 0)
        {
            if(isWild == true)
            {
                player.GetComponent<PlayerStats>().SpecialAdd(0.01f);
            }
            else
            {
                player.GetComponent<PlayerStats>().SpecialAdd(0.02f);
            }
                    
            player.GetComponent<PlayerStats>().ScoreAdd(scorePoints);
            player.GetComponent<PlayerStats>().KillEnemy();
            if(isKeyEnemy)
            {
                player.GetComponent<PlayerStats>().DestroyKeyEnemy();
            }

            Renderer rend = GetComponentInChildren<SpriteRenderer>();
            rend.enabled = false;
            if(isKeyEnemy)
            {
                sound.GetComponent<SoundManager>().PlayDestroySound();
            }
            if(shield != null)
            {
                shield.GetComponent<EnemyShieldContainer>().KillDOTween();
            }
            Destroy(transform.parent.gameObject);
        }
        else
        {
            hitSound.Play(0);
        }
    }

    public void SetEnemyHitPoint(int hp, int specialDmg)
    {
        hitPoints = hp;
        specialDamage = specialDmg;
    }
    
    public void SetOrbType(int color)
    {
        isWild = false;
        orbTypeNeed = color;
    }

    public void SetOrbTypeToWild()
    {
        isWild = true;
    }

    public void SetKeyEnemy(bool keyEmeny)
    {
        isKeyEnemy = keyEmeny;
    }

    public void SetCore(EnemyProperties enemy)
    {
        hitPoints = enemy.hitPoints;
        specialDamage = enemy.specialDamage;
        isKeyEnemy = enemy.isKeyEnemy;
        isWild = enemy.isWild;
        orbTypeNeed = enemy.orbTypeNeed;
        gameObject.GetComponentInChildren<EnemySprite>().ChangeSpriteColor(orbTypeNeed);
    }
}
