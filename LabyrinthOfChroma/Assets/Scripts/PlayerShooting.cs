using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("[Set] Firepoint Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer lineSight;
    [HideInInspector] private RaycastHit2D hit;
    [HideInInspector] private Vector3 laserPoint;


    [Header("[Set] Bullet Settings")]
    [SerializeField] private GameObject[] bulletPrefab;
    [SerializeField] private Gradient[] bulletPrefabLineSightColor;
    [SerializeField] private Gradient[] bulletPrefabLineSightColorFade;
    [SerializeField] private int bulletTypes;
    [SerializeField] public float bulletForce = 20f;

    [Header("[Stat] Bullet State")]
    [SerializeField] public int bulletCurrent = 0;
    [SerializeField] public int bulletShoot = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        bulletCurrent = bulletShoot % bulletTypes;
        
    }

    // Update is called once per frame
    void Update()
    {
        // Ray Casting
        hit = Physics2D.Raycast(firePoint.position, Vector2.up);

        if (hit.collider != null && (hit.collider.tag == "Enemy" || hit.collider.tag == "Orb_Enemy"))
        {
            laserPoint = new Vector3(0.0f, hit.point.y - firePoint.position.y, 0.0f);
            lineSight.SetPosition(1, laserPoint);
            lineSight.colorGradient = bulletPrefabLineSightColor[bulletCurrent];
        }
        else
        {
            laserPoint = new Vector3(0.0f, 5.0f, 0.0f);
            lineSight.SetPosition(1, laserPoint);
            lineSight.colorGradient = bulletPrefabLineSightColorFade[bulletCurrent];
        }
        

        // Shooting
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
            bulletShoot = bulletShoot + 1;
            bulletCurrent = bulletShoot % bulletTypes;
        }
        else if(Input.GetButtonDown("Fire2"))
        {
            bulletShoot = bulletShoot + 1;
            bulletCurrent = bulletShoot % bulletTypes;
        }       
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab[bulletCurrent], firePoint);
        bullet.AddComponent<OrbShooting>();
        bullet.GetComponent<OrbShooting>().orbTypeNumber = bulletCurrent;
        bullet.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        OrbShooting bulletScript = bullet.GetComponent<OrbShooting>();
        bulletScript.orbSpeed = bulletForce;
    }
}