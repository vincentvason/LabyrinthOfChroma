using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool RIGID_BODY = true;
    
    public float moveSpeed = 5f;
    Vector2 movement;
    Vector3 mousePosition;
    Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        if(RIGID_BODY == true){
            rb = GetComponent<Rigidbody2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(RIGID_BODY == false){
            transform.position = Vector2.Lerp(transform.position, mousePosition, moveSpeed);
        }
    } 

    // Update is called once per pre-determined seconds
    void FixedUpdate()
    {
        if(RIGID_BODY == true){
            movement = (mousePosition - transform.position).normalized;
            rb.velocity = new Vector2(movement.x * moveSpeed, movement.y * moveSpeed);
        }
    }
}