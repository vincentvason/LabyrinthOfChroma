using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("[Stat] Player Settings")]
    [SerializeField] public ulong playerScore = 0;
    [SerializeField] public int playerLife;
    [SerializeField] public float playerSpecial;

    [Header("[Stat] Player Stats")]
    [SerializeField] private int numberOrbBreak = 0;
    
    [Header("[Set] Game Rules")]
    [SerializeField] private int playerMaxLife;
    [SerializeField] private ulong lifeScoreBonus = 1000000;
    [SerializeField] private float specialPerOrb;

    void Start()
    {
        playerScore = 0;
        playerSpecial = 0f;
        numberOrbBreak = 0;
    }

    public void OrbDestroy(int numberMatched)
    {
        numberOrbBreak = numberOrbBreak + numberMatched;
        playerSpecial = playerSpecial + (numberMatched * specialPerOrb);
    }

    public void ScoreAdd(int scoreAdded)
    { 
        playerScore = playerScore + (ulong)scoreAdded;
    }

    public void LifeAdd()
    {
        if(playerLife >= playerMaxLife)
        {
            playerScore = playerScore + (ulong)lifeScoreBonus; 
        }
        else
        {
            playerLife = playerLife + 1;
        }
    }

    public void LifeLose()
    {
        if(playerLife < 0)
        {
            // Go to Game Over Scene       
        }
        else
        {
            playerLife = playerLife - 1;
        }
    }
}