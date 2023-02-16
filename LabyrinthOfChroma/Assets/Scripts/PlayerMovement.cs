using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("[Set] Game Manager")]
    [HideInInspector] private GameObject playerStats;
    [HideInInspector] private GameObject sound;
    
    
    [Header("[Set] Player Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [HideInInspector] private Vector2 movement;
    [HideInInspector] private Vector3 mousePosition;
    
    [Header("[Stat] Invicibility State")]
    [SerializeField] private bool isInvisible;

    [Header("[Set] Invicibility Setting")]
    [SerializeField] private AudioSource hitSound;
    [SerializeField] private Color flashColor;
    [SerializeField] private Color regularColor;
    [SerializeField] private float flashDuration;
    [SerializeField] private int numberOfFlashes;
    [SerializeField] private Collider2D triggerCollider;
    [SerializeField] private SpriteRenderer mySprite;
    
    // Start is called before the first frame update
    void Start()
    {
        playerStats = GameObject.Find("Game System");
        sound = GameObject.Find("Sound System");
        StartCoroutine("FlashCoroutine");
    }

    // Update is called once per frame
    void Update()
    {
        if(playerStats.gameObject.GetComponent<PlayerStats>().playerLife >= 0)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
        }
    } 

    // Update is called once per pre-determined seconds
    void FixedUpdate()
    {
        transform.position = Vector2.Lerp(transform.position, mousePosition, moveSpeed);

        if(transform.position.x < -7)
        {
            transform.position = new Vector2(-7, transform.position.y);
        }
        else if(transform.position.x > 7)
        {
            transform.position = new Vector2(7, transform.position.y);
        }
        else if(transform.position.y < -5)
        {
            transform.position = new Vector2(transform.position.x, -5);
        }
        else if(transform.position.y > 5)
        {
            transform.position = new Vector2(transform.position.x, 5);
        }
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Orb_Enemy" || collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Bullet_Enemy")
        {
            if(playerStats.gameObject.GetComponent<PlayerStats>().playerLife >= 0)
            {
                sound.GetComponent<SoundManager>().PlayDestroySound();
            }
            playerStats.gameObject.GetComponent<PlayerStats>().LifeLose();
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

