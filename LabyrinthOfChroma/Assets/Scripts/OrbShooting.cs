using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbShooting : MonoBehaviour
{
    [SerializeField] public int orbTypeNumber;
    [SerializeField] private float borderYPosition = 6f;
    [SerializeField] public float orbSpeed = 1f;
    [SerializeField] private bool collided;

    // Start is called before the first frame update
    void Start()
    {
        collided = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.tag == "Orb_Player" && gameObject.transform.position.y > borderYPosition){
            Destroy(gameObject);
        }
        transform.Translate(Vector3.up * Time.deltaTime * orbSpeed, Camera.main.transform);
    }

    // On Collision Enter 2D is called when collide
    
    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Orb_Enemy" && collided == false){
            collided = true;

            this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            this.GetComponent<Rigidbody2D>().angularVelocity = 0f;

            GameObject enemyOrb = collision.gameObject;
            Debug.Log("Enemy Orb: "+enemyOrb);
            GameObject enemyOrbContainer = enemyOrb.transform.parent.gameObject;
            Debug.Log("Enemy Orb Container: "+enemyOrbContainer);
            EnemyShieldContainer enemyOrbContainerScript = enemyOrbContainer.gameObject.GetComponentInParent<EnemyShieldContainer>();

            int currentIndex = enemyOrb.transform.GetSiblingIndex();
            Debug.Log("CurrentIndex:"+currentIndex);

            enemyOrbContainerScript.AddNewBall(this.gameObject, currentIndex);


            gameObject.tag = "Orb_Enemy";
            this.gameObject.GetComponent<OrbShooting>().enabled = false;
        }
        else if(gameObject.tag == "Orb_Player" && collision.gameObject.tag == "Enemy" && collided == false)
        {
            collided = true;
            Destroy(gameObject);
        }
    }
   
}
