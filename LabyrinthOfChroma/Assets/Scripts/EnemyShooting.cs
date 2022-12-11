using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [SerializeField] EnemyBulletPool bulletPoolInstance;

    [SerializeField] private Transform bulletParent;
    [SerializeField] private int bulletLines = 10;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float fireSeconds = 2f;

    [SerializeField] private float lastTimeShooting = 0f;
    [SerializeField] private float currentTimeShooting = 0f;
    
    [SerializeField] private float startAngle = 90f;
    [SerializeField] private float endAngle = 270f;
    
    [SerializeField] private Transform playerLocation;
    [SerializeField] private bool trackPlayer;
    
    [SerializeField] float angleBetweenPlayer;
    [HideInInspector] Vector2 bulletMoveDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTimeShooting += Time.deltaTime;

        if(trackPlayer == true)
        {
            GetPlayerAngle();
        }        

        if(currentTimeShooting - lastTimeShooting > fireSeconds)
        {
            Fire();
            lastTimeShooting = currentTimeShooting;
        }
    }

    private void GetPlayerAngle()
    {
        Vector3 dir = gameObject.transform.position - playerLocation.position;
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

            GameObject bul = bulletPoolInstance.GetBullet(bulletParent);
            bul.transform.position = transform.position;
            bul.transform.rotation = transform.rotation;
            bul.SetActive(true);
            bul.GetComponent<EnemyBullet>().SetMove(bulDir, bulletSpeed);

            angle += angleStep;
        }
    }
}
