using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageScoreManagement : MonoBehaviour
{
    public Text stageText;
    public int stage;
    
    public Text scoreText;
    public uint score;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
    }

    public void AddScore(uint addedScore)
    {
        score += addedScore;
    }
}
