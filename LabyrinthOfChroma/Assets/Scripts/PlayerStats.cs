using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("[Stat] Player Settings")]
    [SerializeField] public ulong playerScore = 0;
    [SerializeField] public int playerLife;
    [SerializeField] public float playerSpecial = 0.0f;

    [Header("[Stat] Player Stats")]
    [SerializeField] private int numberOrbBreak = 0;
    
    [Header("[Set] Game Rules")]
    [SerializeField] public int playerMaxLife;
    [SerializeField] public float playerMaxSpecial = 3.0f;
    [SerializeField] private ulong lifeScoreBonus = 1000000;
    [SerializeField] private float specialPerOrb;

    void Start()
    {

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

    public void SpecialAdd(float specialAdded)
    { 
        if((playerSpecial + specialAdded) > playerMaxSpecial)
        {
            playerSpecial = playerMaxSpecial;
        }
        else
        {
            playerSpecial = playerSpecial + specialAdded;
        }
    }

    public void SpecialUsed()
    { 
        if(playerSpecial < 1.0f)
        {
            // Do nothing.       
        }
        else
        {
            playerSpecial = playerSpecial - 1.0f;
        }
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