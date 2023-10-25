using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [HideInInspector] private GameObject bulletPoolInstance;

    [HideInInspector] private GameObject bulletParent;
    [SerializeField] private int bulletLines = 10;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float fireSeconds = 2f;

    [SerializeField] private float lastTimeShooting = 0f;
    [SerializeField] private float currentTimeShooting = 0f;
    
    [SerializeField] private float startAngle = 90f;
    [SerializeField] private float endAngle = 270f;
    
    [HideInInspector] private GameObject playerLocation;
    [SerializeField] private bool trackPlayer = false;
    
    [SerializeField] float angleBetweenPlayer;
    [HideInInspector] Vector2 bulletMoveDirection;

    [SerializeField] private AudioSource shootingSound;

    // Start is called before the first frame update
    void Start()
    {
        bulletPoolInstance = GameObject.Find("Bullet Pool Scene");
        bulletParent = GameObject.Find("Bullet Pool Scene");

        playerLocation = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        currentTimeShooting += Time.deltaTime;

        if(trackPlayer == true)
        {
            GetPlayerAngle();
        }        

        if(currentTimeShooting - lastTimeShooting > fireSeconds && gameObject.transform.position.y > -4)
        {
            Fire();
            shootingSound.Play(0);
            lastTimeShooting = currentTimeShooting;
        }
    }

    private void GetPlayerAngle()
    {
        Vector3 dir = gameObject.transform.position - playerLocation.transform.position;
        angleBetweenPlayer = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    private void Fire()
    {
        float angleStep = (endAngle - startAngle) / (bulletLines-1);
        float angle = startAngle;

        for(int index = 0; index < bulletLines; index++)
        {
            float bulDirX, bulDirY;

            if(trackPlayer)
            {
                bulDirX = transform.position.x + Mathf.Sin(((angle - angleBetweenPlayer - 90) * Mathf.PI) / 180.0f);
                bulDirY = transform.position.y + Mathf.Cos(((angle - angleBetweenPlayer - 90) * Mathf.PI) / 180.0f);
            }
            else
            {
                bulDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180.0f);
                bulDirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180.0f);
            }
            
            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0.0f);
            Vector2 bulDir = (bulMoveVector - transform.position).normalized;

            GameObject bul = bulletPoolInstance.GetComponent<EnemyBulletPool>().GetBullet(bulletParent.transform);
            bul.transform.position = transform.position;
            bul.transform.rotation = transform.rotation;
            bul.SetActive(true);
            bul.GetComponent<EnemyBullet>().SetMove(bulDir, bulletSpeed);

            angle += angleStep;
        }
    }

    public void SetBulletSpeed(float newBulletSpeed, float newFireSeconds)
    {
        bulletSpeed = newBulletSpeed;
        fireSeconds = newFireSeconds;
    }

    public void SetBulletSpreading(int newBulletLines, bool newTrackPlayer, float newStartAngle, float newEndAngle)
    {
        bulletLines = newBulletLines;
        trackPlayer = newTrackPlayer;
        startAngle = newStartAngle;
        endAngle = newEndAngle;
    }

    public void SetShooting(EnemyProperties enemy)
    {
        bulletLines = enemy.bulletLines;
        trackPlayer = enemy.trackPlayer;
        startAngle = enemy.startAngle;
        endAngle = enemy.endAngle;

        bulletSpeed = enemy.bulletSpeed;
        fireSeconds = enemy.fireSeconds;
    }
}
