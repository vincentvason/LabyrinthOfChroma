using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject[] bulletPrefab;
    public int bulletTypes;

    public int bulletCurrent;
    public int bulletNext;

    public float bulletForce = 20f;
    
    // Start is called before the first frame update
    void Start()
    {
        bulletCurrent = Random.Range(0, bulletTypes);
        bulletNext = Random.Range(0, bulletTypes);
        Debug.Log("C:"+bulletCurrent+",N:"+bulletNext);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
            bulletCurrent = bulletNext;
            bulletNext = Random.Range(0, bulletTypes);
            Debug.Log("C:"+bulletCurrent+",N:"+bulletNext);
        }
        else if(Input.GetButtonDown("Fire2"))
        {
            int temp;
            temp = bulletCurrent;
            bulletCurrent = bulletNext;
            bulletNext = temp;
            Debug.Log("C:"+bulletCurrent+",N:"+bulletNext);
        }       
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab[bulletCurrent], firePoint.position, firePoint.rotation);
        bullet.AddComponent<OrbShooting>();
        bullet.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        OrbShooting bulletScript = bullet.GetComponent<OrbShooting>();
        bulletScript.orbSpeed = bulletForce;
    }
}
