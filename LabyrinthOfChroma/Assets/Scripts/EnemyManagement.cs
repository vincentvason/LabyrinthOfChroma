using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManagement : MonoBehaviour
{
    [Header("[Set] Game Stats")]
    [HideInInspector] private GameObject playerStats;
    [HideInInspector] private GameObject player;
    [SerializeField] private int wave;

    [SerializeField] private float delayOneOnOne;
    [SerializeField] private float delayHorde;

    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletDelay;
    [SerializeField] private int bulletLine;
    [SerializeField] private float stillProbability;
    [SerializeField] private int bulletAngle;

    [SerializeField] private int hpBasic;
    [SerializeField] private int spDmgBasic;
    [SerializeField] private int numberOfEnemy;
    [SerializeField] private int numberOfEnemyInHorde;
    [SerializeField] private int numberOfHorde;
    [SerializeField] private int numberOfColor;
    [SerializeField] private float wildProbability;

    [SerializeField] private int bossType;
    [SerializeField] private int hpBoss;
    [SerializeField] private int spDmgBoss;
    [SerializeField] private float bonusTime;
    [SerializeField] private int bonusValue;
    
    [Header("[Set] Ememy Type")]
    [SerializeField] private GameObject enemyBasic;
    [SerializeField] private List<GameObject> enemyBoss;



    // Start is called before the first frame update
    IEnumerator Start()
    {
        playerStats = GameObject.Find("Game System");
        player = GameObject.Find("Player");

        for(wave = 1; true; wave++)
        {
            ParameterSetting();
            player.GetComponent<PlayerShooting>().SetColor(numberOfColor);

            Debug.Log("wave: "+wave);
            playerStats.GetComponent<PlayerStats>().NextWave();

            if(wave % 5 == 0)
            {
                SpawnBoss();
                playerStats.GetComponent<PlayerStats>().EnableBonus(bonusTime, bonusValue);
                while(playerStats.GetComponent<PlayerStats>().keyEnemy != 0)
                {
                    yield return new WaitForSeconds(delayOneOnOne);
                }
            }
            else if(wave % 2 == 0)
            {
                yield return EvenWave();
            }
            else
            {
                yield return OddWave();
            }
            
        }
    }

    void ParameterSetting()
    {
        delayOneOnOne = Mathf.Clamp((0.000004611f*wave*wave) - (0.005516f*wave)+1.006f, 0.1f, 1f);
        delayHorde = Mathf.Clamp((0.00001372f*wave*wave) - (0.01654f*wave)+3.017f, 2f, 6f);

        bulletSpeed = Mathf.Clamp((-0.00002831f*wave*wave) + (0.05336f*wave)+4.947f, 5f, 30f);
        bulletDelay = Mathf.Clamp((-0.000004166f*wave*wave) + (0.005471f*wave)+2.005f, 0.7f, 2f);

        stillProbability = Mathf.Clamp((0.000002983f*wave*wave) - (0.003837f*wave)+0.8538f, 0f, 1f);
        wildProbability = Mathf.Clamp((0.000002983f*wave*wave) - (0.003837f*wave)+0.8538f, 0f, 1f);
        
       
        hpBoss = (wave/5)+1;
        if(wave >= 65)
        {
            bossType = 2;
            hpBasic = (int)Mathf.Clamp((0.000007104f*wave*wave) - (0.02219f*wave)+0.7104f, 1f, 30f);
            hpBoss = (int)Mathf.Clamp((-0.0002442f*wave*wave) - (0.2575f*wave)+16.69f, 20f, 300f);
            spDmgBasic = 1;
            spDmgBoss = (int)Mathf.Clamp((-0.00005823f*wave*wave) - (0.06405f*wave)+4.177f, 5f, 10f);

            numberOfEnemy = (int)Mathf.Clamp((-0.0002376f*wave*wave) - (0.4280f*wave)+9.572f, 10f, 50f);
            numberOfEnemyInHorde = (int)Mathf.Clamp((-0.00002831f*wave*wave) - (0.05336f*wave)+4.947f, 5f, 30f);

            numberOfHorde = 10;
            numberOfColor = 6;
            bonusTime = Mathf.Clamp(((0.0004321f*wave*wave) - (0.5086f*wave)+106.5f)*(0.5f+(0.5f*bossType)), 15f, 150f);
            bonusValue = (int)Mathf.Clamp((1056*wave*wave) - (61890f*wave) + 3626000, 3000000f, 999999999f);
            bulletLine = (int)Mathf.Clamp((0.000007130f*wave*wave) - (0.001176f*wave)+3.046f, 3f, 9f);
            bulletAngle = (int)Mathf.Clamp((-0.0007741f*wave*wave) - (0.9849f*wave)+30.75f, 30f, 180f);

        }
        else
        {
            bossType = ((wave/5)-1)%3;
            hpBasic = 1;
            hpBoss = 20;
            spDmgBasic = 1;
            spDmgBoss = 5;

            numberOfEnemy = (int)Mathf.Clamp((-0.0002376f*wave*wave) - (0.4280f*wave)+9.572f, 10f, 50f);
            numberOfEnemyInHorde = (int)Mathf.Clamp((-0.00002831f*wave*wave) - (0.05336f*wave)+4.947f, 5f, 30f);

            numberOfHorde = 5;
            numberOfColor = 3+((wave-1)/5)/3;
            bonusTime = 45f+(5f*(wave/5));
            bonusValue = 500000 +(250000*(wave/5));
            bulletLine = (int)1;
            bulletAngle = (int)0;
        }
    }

    void SpawnBasic()
    {
        AnimationCurve moveX = new AnimationCurve();
        AnimationCurve moveY = new AnimationCurve();

        moveX.AddKey(0f,0f);
        moveY.AddKey(0f,-3f);
        moveY.AddKey(0.5f,-3f);
        moveY.AddKey(2f,-1f);

        int selectedColor = Random.Range(0, numberOfColor);

        Vector3 position = new Vector3(Random.Range(-7.0f, 7.0f), 5.5f, 0.0f);
        GameObject enemy = Instantiate(enemyBasic, position, Quaternion.identity, gameObject.transform);
        enemy.GetComponent<EnemyMovement>().SetMovement(moveX,moveY);
        enemy.GetComponentInChildren<EnemyShooting>().SetBulletSpeed(bulletSpeed,bulletDelay);
        if(Random.Range(0f, 1f) > stillProbability)
        {
            enemy.GetComponentInChildren<EnemyShooting>().SetBulletSpreading(bulletLine,true,0f-(bulletAngle/2),0f+(bulletAngle/2));
        }
        else
        {
            enemy.GetComponentInChildren<EnemyShooting>().SetBulletSpreading(bulletLine,false,180f-(bulletAngle/2),180f+(bulletAngle/2));
        }
        enemy.GetComponentInChildren<EnemyCore>().SetEnemyHitPoint(hpBasic,spDmgBasic);
        if(Random.Range(0f, 1f) > wildProbability)
        {
            enemy.GetComponentInChildren<EnemySprite>().ChangeSpriteColor(selectedColor);
            enemy.GetComponentInChildren<EnemyCore>().SetOrbType(selectedColor);
        }
    }


    IEnumerator OddWave()
    {        
        for(int i = 0; i < numberOfEnemy; i++){
            SpawnBasic();
            yield return new WaitForSeconds(delayOneOnOne);
        }
    }

    IEnumerator EvenWave()
    {
        for(int j = 0; j < numberOfHorde; j++)
        {
            for(int i = 0; i < numberOfEnemyInHorde; i++)
            {
                SpawnBasic();
            }
            yield return new WaitForSeconds(delayHorde);
        }
    }

    void SpawnBoss()
    {
        AnimationCurve moveX = new AnimationCurve();
        AnimationCurve moveY = new AnimationCurve();

        moveX.AddKey(0f,0f);
        moveY.AddKey(0f,-2.8f);
        moveY.AddKey(0.5f,-2.8f);
        moveY.AddKey(2f,0f);

        Vector3 position = new Vector3(0f, 5.5f, 0.0f);
        GameObject enemy = Instantiate(enemyBoss[bossType], position, Quaternion.identity, gameObject.transform);
        enemy.GetComponent<EnemyMovement>().SetMovement(moveX,moveY);
        enemy.GetComponentInChildren<EnemyShooting>().SetBulletSpeed(bulletSpeed,bulletDelay);
        enemy.GetComponentInChildren<EnemyShooting>().SetBulletSpreading(bulletLine+2,true,0f-(bulletAngle/2),0f+(bulletAngle/2));
        enemy.GetComponentInChildren<EnemyCore>().SetKeyEnemy(true);
        enemy.GetComponentInChildren<EnemyCore>().SetEnemyHitPoint(hpBoss,spDmgBoss);
        enemy.GetComponentInChildren<EnemyShieldContainer>().SetOrbColor(numberOfColor);
        playerStats.GetComponent<PlayerStats>().AddKeyEnemy();
    }
}
