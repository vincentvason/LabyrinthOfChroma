using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyMovement2 : MonoBehaviour
{
    [Header("[Set] Player Tracking Location")]
    [HideInInspector] private Transform player;

    [Header("[Set] Enemy Speed Setting")]
    [SerializeField] private List<Vector3> waypoints;
    [SerializeField] private float duration;


    [SerializeField] private int loop = 1;
    [SerializeField] private bool destroyWhenOutside;
    [SerializeField] private PathType pathType = PathType.Linear;
    [SerializeField] private Ease ease = Ease.Linear;

    public void SetMovement(List<Vector3> waypoints, float duration, int loop = 1, bool destroyWhenOutside = true, PathType pathType = PathType.Linear, Ease ease = Ease.Linear)
    {
        this.waypoints = waypoints;
        this.duration = duration;
        this.loop = loop;
        this.destroyWhenOutside = destroyWhenOutside;
        this.pathType = pathType;
        this.ease = ease;

        gameObject.transform.DOPath(waypoints.ToArray(), duration, pathType).SetEase(ease).SetLoops(loop);
    }

    public void SetMovement(EnemyProperties enemy)
    {
        this.waypoints = enemy.waypoints;
        this.duration = enemy.duration;
        this.loop = enemy.loop;
        this.destroyWhenOutside = enemy.destroyWhenOutside;
        this.pathType = enemy.pathType;
        this.ease = enemy.ease;

        gameObject.transform.DOPath(enemy.waypoints.ToArray(), enemy.duration, enemy.pathType).SetEase(ease).SetLoops(loop);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject cam = GameObject.Find("Main Camera");

        Vector3 bottomLeftPosition = cam.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0, 0, cam.GetComponent<Camera>().nearClipPlane));
        Vector3 topRightPosition = cam.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(1, 1, cam.GetComponent<Camera>().nearClipPlane));

        if((gameObject.transform.position == waypoints[waypoints.Count-1]) && destroyWhenOutside)
        {
            if(transform.position.x < bottomLeftPosition.x - 0.5f)
            {
                DOTween.KillAll();
                Destroy(gameObject);
            }
            else if(transform.position.x > topRightPosition.x + 0.5f)
            {
                DOTween.KillAll();
                Destroy(gameObject);
            }
            else if(transform.position.y < bottomLeftPosition.y - 0.5f)
            {
                DOTween.KillAll();
                Destroy(gameObject);
            }
            else if(transform.position.y > topRightPosition.y + 0.5f)
            {
                DOTween.KillAll();
                Destroy(gameObject);
            }
        }
    }
}
