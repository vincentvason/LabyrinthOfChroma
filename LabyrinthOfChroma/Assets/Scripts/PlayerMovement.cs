using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("[Set] Game Manager")]
    [HideInInspector] private GameObject camera;
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
        camera = GameObject.Find("Main Camera");
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
        //Current Position
        transform.position = Vector2.Lerp(transform.position, mousePosition, moveSpeed);

        Vector3 bottomLeftPosition = camera.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0, 0, camera.GetComponent<Camera>().nearClipPlane));
        Vector3 topRightPosition = camera.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(1, 1, camera.GetComponent<Camera>().nearClipPlane));

        if(transform.position.x < bottomLeftPosition.x)
        {
            transform.position = new Vector2(bottomLeftPosition.x, transform.position.y);
        }
        else if(transform.position.x > topRightPosition.x)
        {
            transform.position = new Vector2(topRightPosition.x, transform.position.y);
        }
        else if(transform.position.y < bottomLeftPosition.y)
        {
            transform.position = new Vector2(transform.position.x, bottomLeftPosition.y);
        }
        else if(transform.position.y > topRightPosition.y)
        {
            transform.position = new Vector2(transform.position.x, topRightPosition.y);
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

