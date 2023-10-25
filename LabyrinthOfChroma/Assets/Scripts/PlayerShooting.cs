using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("[Set] Player Stats")]
    [HideInInspector] private GameObject playerStats;
    [SerializeField] private Transform scene;

    [Header("[Set] Firepoint Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer lineSight;
    [HideInInspector] private RaycastHit2D hit;
    [HideInInspector] private Vector3 laserPoint;

    [Header("[Set] Firepoint Audio")]
    [SerializeField] private AudioSource shootSound;

    [Header("[Set] Bullet Settings")]
    [SerializeField] private SpriteRenderer capsuleColor;
    [SerializeField] private GameObject[] bulletPrefab;
    [SerializeField] private GameObject bulletSpecialPrefab;
    [SerializeField] private Color[] bulletColor;
    [HideInInspector] private int bulletTypes = 3;
    [SerializeField] public float bulletForce = 20f;

    [Header("[Stat] Bullet State")]
    [HideInInspector] private const int SPECIAL_BULLET = 99;
    [SerializeField] public int bulletCurrent = 0;
    [SerializeField] public int bulletShoot = 0;
    [SerializeField] private float timeElpased;
    [SerializeField] private Gradient gradient;

    
    // Start is called before the first frame update
    void Start()
    {
        playerStats = GameObject.Find("Game System");

        bulletCurrent = bulletShoot % bulletTypes;   
    }

    // Update is called once per frame
    void Update()
    {
        // Timer
        timeElpased += Time.deltaTime;


        // Ray Casting
        hit = Physics2D.Raycast(firePoint.position, Vector2.up);
        gradient = new Gradient();

        laserPoint = new Vector3(0.0f, hit.point.y - firePoint.position.y, 0.0f);

        if(bulletCurrent == SPECIAL_BULLET && hit.collider != null && (hit.collider.tag == "Enemy" || hit.collider.tag == "Orb_Enemy"))
        {
            laserPoint = new Vector3(0.0f, hit.point.y - firePoint.position.y, 0.0f);
            
            gradient.SetKeys(
                new GradientColorKey[]  {
                                            new GradientColorKey(Color.red, (((timeElpased/2) + (0.0f/6.0f)) % 1.0f)), 
                                            new GradientColorKey(Color.yellow, (((timeElpased/2) + (1.0f/6.0f)) % 1.0f)), 
                                            new GradientColorKey(Color.green, (((timeElpased/2) + (2.0f/6.0f)) % 1.0f)), 
                                            new GradientColorKey(Color.cyan, (((timeElpased/2) + (3.0f/6.0f)) % 1.0f)), 
                                            new GradientColorKey(Color.blue, (((timeElpased/2) + (4.0f/6.0f)) % 1.0f)), 
                                            new GradientColorKey(Color.magenta, (((timeElpased/2) + (5.0f/6.0f)) % 1.0f))
                                        },
                new GradientAlphaKey[]  {
                                            new GradientAlphaKey(0.8f, 0.0f), 
                                            new GradientAlphaKey(0.8f, 0.5f), 
                                            new GradientAlphaKey(0.8f, 1.0f)
                                        }
            );
            lineSight.colorGradient = gradient;
        }
        else if(bulletCurrent == SPECIAL_BULLET)
        {
            laserPoint = new Vector3(0.0f, 5.0f, 0.0f);

            gradient.SetKeys(
                new GradientColorKey[]  {
                                            new GradientColorKey(Color.red, (((timeElpased/2) + (0.0f/6.0f)) % 1.0f)), 
                                            new GradientColorKey(Color.yellow, (((timeElpased/2) + (1.0f/6.0f)) % 1.0f)), 
                                            new GradientColorKey(Color.green, (((timeElpased/2) + (2.0f/6.0f)) % 1.0f)), 
                                            new GradientColorKey(Color.cyan, (((timeElpased/2) + (3.0f/6.0f)) % 1.0f)), 
                                            new GradientColorKey(Color.blue, (((timeElpased/2) + (4.0f/6.0f)) % 1.0f)), 
                                            new GradientColorKey(Color.magenta, (((timeElpased/2) + (5.0f/6.0f)) % 1.0f))
                                        },
                new GradientAlphaKey[]  {
                                            new GradientAlphaKey(0.8f, 0.0f), 
                                            new GradientAlphaKey(0.8f, 0.5f), 
                                            new GradientAlphaKey(0.0f, 1.0f)
                                        }
            );
            lineSight.colorGradient = gradient;            
        }
        else if(playerStats.gameObject.GetComponent<PlayerStats>().playerSpecial >= 1.0f && hit.collider != null && (hit.collider.tag == "Enemy" || hit.collider.tag == "Orb_Enemy"))
        {
            laserPoint = new Vector3(0.0f, hit.point.y - firePoint.position.y, 0.0f);
            
            gradient.SetKeys(
                new GradientColorKey[]  {
                                            new GradientColorKey(bulletColor[bulletCurrent], 0.0f),
                                            new GradientColorKey(Color.white, Mathf.Clamp((timeElpased/2) % 1.0f, 0.001f, 0.999f)),
                                            new GradientColorKey(bulletColor[bulletCurrent], 1.0f)
                                        },
                new GradientAlphaKey[]  {
                                            new GradientAlphaKey(0.8f, 0.0f),
                                            new GradientAlphaKey(0.8f, 0.5f),
                                            new GradientAlphaKey(0.8f, 1.0f)
                                        }
            );
            lineSight.colorGradient = gradient;
        }
        else if(playerStats.gameObject.GetComponent<PlayerStats>().playerSpecial >= 1.0f)
        {
            laserPoint = new Vector3(0.0f, 5.0f, 0.0f);

            gradient.SetKeys(
                new GradientColorKey[]  {
                                            new GradientColorKey(bulletColor[bulletCurrent], 0.0f),
                                            new GradientColorKey(Color.white, Mathf.Clamp((timeElpased/2) % 1.0f, 0.001f, 0.999f)),
                                            new GradientColorKey(bulletColor[bulletCurrent], 1.0f)
                                        },
                new GradientAlphaKey[]  {
                                            new GradientAlphaKey(0.8f, 0.0f),
                                            new GradientAlphaKey(0.8f, 0.5f),
                                            new GradientAlphaKey(0.0f, 1.0f)
                                        }
            );
            lineSight.colorGradient = gradient;
        }
        else if (hit.collider != null && (hit.collider.tag == "Enemy" || hit.collider.tag == "Orb_Enemy"))
        {
            laserPoint = new Vector3(0.0f, hit.point.y - firePoint.position.y, 0.0f);

            gradient.SetKeys(
                new GradientColorKey[]  {
                                            new GradientColorKey(bulletColor[bulletCurrent], 0.0f),
                                            new GradientColorKey(bulletColor[bulletCurrent], 1.0f)
                                        },
                new GradientAlphaKey[]  {
                                            new GradientAlphaKey(0.8f, 0.0f),
                                            new GradientAlphaKey(0.8f, 0.5f),
                                            new GradientAlphaKey(0.8f, 1.0f)
                                        }
            );
            lineSight.colorGradient = gradient;
        }
        else
        {
            laserPoint = new Vector3(0.0f, 5.0f, 0.0f);
            
            gradient.SetKeys(
                new GradientColorKey[]  {
                                            new GradientColorKey(bulletColor[bulletCurrent], 0.0f),
                                            new GradientColorKey(bulletColor[bulletCurrent], 1.0f)
                                        },
                new GradientAlphaKey[]  {
                                            new GradientAlphaKey(0.8f, 0.0f),
                                            new GradientAlphaKey(0.8f, 0.5f),
                                            new GradientAlphaKey(0.0f, 1.0f)
                                        }
            );
            lineSight.colorGradient = gradient;
        }

        if(bulletCurrent == SPECIAL_BULLET)
        {
            float key = timeElpased % 1;
            float r = key;
            float g = 1 - Mathf.Abs((key - 0.5f) * 2);
            float b = 1 - key;

            capsuleColor.color = new Color(r, g, b, 1.0f);
        }
        else
        {
            capsuleColor.color = bulletColor[bulletCurrent];
        }

        
        for(int i = 1; i < 20; i++)
        {
            lineSight.SetPosition(i, laserPoint*(i/19.0f));
        }


        if(playerStats.gameObject.GetComponent<PlayerStats>().playerLife >= 0)
        {
            // Shooting
            if(Input.GetMouseButtonDown(0))
            {
                Shoot();
                if(bulletCurrent == SPECIAL_BULLET)
                {
                    playerStats.gameObject.GetComponent<PlayerStats>().SpecialUsed();
                }
                bulletShoot = bulletShoot + 1;
                bulletCurrent = bulletShoot % bulletTypes;
            }
            else if(Input.GetMouseButtonDown(1))
            {
                bulletShoot = bulletShoot + 1;
                bulletCurrent = bulletShoot % bulletTypes;
            }
            else if((Input.GetMouseButtonDown(2) || Input.GetKeyDown("space")) && bulletCurrent == SPECIAL_BULLET)
            {
                bulletCurrent = bulletShoot % bulletTypes;
            }
            else if((Input.GetMouseButtonDown(2) || Input.GetKeyDown("space")) && playerStats.gameObject.GetComponent<PlayerStats>().playerSpecial >= 1.0f)
            {
                bulletCurrent = SPECIAL_BULLET;
            }        
        }
   
    }

    void Shoot()
    {
        GameObject bullet;
        if(bulletCurrent == SPECIAL_BULLET)
        {
            bullet = Instantiate(bulletSpecialPrefab, firePoint);
        }
        else
        {
            bullet = Instantiate(bulletPrefab[bulletCurrent], firePoint);
        }
        bullet.transform.SetParent(scene);
        bullet.AddComponent<OrbShooting>();
        bullet.GetComponent<OrbShooting>().orbTypeNumber = bulletCurrent;
        bullet.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        OrbShooting bulletScript = bullet.GetComponent<OrbShooting>();
        bulletScript.orbSpeed = bulletForce;
        shootSound.Play(0);
    }

    public void SetColor(int numberOfColorType)
    {
        bulletTypes = numberOfColorType;
    }
}