using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool invisibie = true;
    Vector2 movement;
    Vector3 mousePosition;
    
    // Start is called before the first frame update
    void Start()
    {
        invisibie = true;
        // Invoke("InvisibleOff", 3f);
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = Vector2.Lerp(transform.position, mousePosition, moveSpeed);
    } 

    // Update is called once per pre-determined seconds
    void FixedUpdate()
    {
        movement = (mousePosition - transform.position).normalized;
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Orb_Enemy" || collision.gameObject.tag == "Enemy"){
        
        }
    }

    void InvisibieOff()
    {
        invisibie = false;
    }
}

