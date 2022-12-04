using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [HideInInspector] private Vector2 moveDirection;
    [HideInInspector] private float moveSpeed = 5f;

    // Start is called before the first frame update
    void OnEnable()
    {
        Invoke("Destroy", 3f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    public void SetMove(Vector2 dir, float speed)
    {
        moveDirection = dir;
        moveSpeed = speed;
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
