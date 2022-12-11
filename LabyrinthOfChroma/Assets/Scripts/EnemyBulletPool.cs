using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletPool : MonoBehaviour
{
    [SerializeField] private GameObject pooledBullet;
    [SerializeField] bool notEnoughBulletsInPool = true;

    [HideInInspector] private List<GameObject> bullets;

    void Start()
    {
        bullets = new List<GameObject>();
    }

    public GameObject GetBullet(Transform parent)
    {
        if(bullets.Count > 0)
        {
            for(int i = 0; i < bullets.Count; i++)
            {
                if(!bullets[i].activeInHierarchy)
                {
                    return bullets[i];
                }
            }
        }

        if(notEnoughBulletsInPool)
        {
            GameObject bul = Instantiate(pooledBullet, parent);
            bul.SetActive(false);
            bullets.Add(bul);
            return bul;
        }
        
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
