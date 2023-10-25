using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManagement2 : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<EnemySpawner>().SpawnTutorialWave0();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
