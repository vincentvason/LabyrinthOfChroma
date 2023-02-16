using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private float moveSpeed = 5f;

    // Start is called before the first frame update
    void OnEnable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.position.y < -6 && gameObject.activeSelf == true)
        {
            gameObject.SetActive(false);
        }
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    public void SetMove(Vector2 dir, float speed)
    {
        moveDirection = dir;
        moveSpeed = speed;
    }
}
