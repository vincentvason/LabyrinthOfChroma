using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("[Set] Game Manager")]
    [SerializeField] private PlayerStats playerStats;
    
    [Header("[Set] Player Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [HideInInspector] private Vector2 movement;
    [HideInInspector] private Vector3 mousePosition;
    
    [Header("[Stat] Invicibility State")]
    [SerializeField] private bool isInvisible;

    [Header("[Set] Invicibility Setting")]
    [SerializeField] private Color flashColor;
    [SerializeField] private Color regularColor;
    [SerializeField] private float flashDuration;
    [SerializeField] private int numberOfFlashes;
    [SerializeField] private Collider2D triggerCollider;
    [SerializeField] private SpriteRenderer mySprite;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("FlashCoroutine");
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        
    } 

    // Update is called once per pre-determined seconds
    void FixedUpdate()
    {
        transform.position = Vector2.Lerp(transform.position, mousePosition, moveSpeed);
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Orb_Enemy" || collision.gameObject.tag == "Enemy")
        {
            playerStats.LifeLose();
            StartCoroutine("FlashCoroutine");
        }
    }

    private IEnumerator FlashCoroutine()
    {
        isInvisible = true;
        triggerCollider.enabled = !isInvisible;
        for(int times = 0;times < numberOfFlashes; times++)
        {
            mySprite.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            mySprite.color = regularColor;
            yield return new WaitForSeconds(flashDuration);
        }
        isInvisible = false;
        triggerCollider.enabled = !isInvisible;
        yield return null;
    }
}

