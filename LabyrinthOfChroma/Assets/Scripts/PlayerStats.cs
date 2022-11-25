using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("[Stat] Player Settings")]
    [SerializeField] public ulong playerScore = 0;
    [SerializeField] public int playerLife = 2;
    [SerializeField] public float playerSpecial = 0f;

    [Header("[Stat] Player Stats")]
    [SerializeField] public int numberOrbBreak = 0;
    [SerializeField] public float specialPerOrb = 0f;

    [Header("[Stat] Game Rules")]
    [SerializeField] public int playerMaxLife = 6;
    [SerializeField] public ulong lifeScoreBonus = 1000000;

    void Start()
    {
        playerScore = 0;
        playerSpecial = 0f;
        numberOrbBreak = 0;
        specialPerOrb = 0f;
    }

    public void OrbDestroy(int numberMatched){
        numberOrbBreak = numberOrbBreak + numberMatched;
        playerSpecial = playerSpecial + (numberMatched * specialPerOrb);
    }

    public void ScoreAdd(int scoreAdded){ 
        playerScore = playerScore + (ulong)scoreAdded;
    }

    public void LifeAdd(){
        if(playerLife >= playerMaxLife){
            playerScore = playerScore + (ulong)lifeScoreBonus; 
        }
        else{
            playerLife = playerLife + 1;
        }
    }

    public void LifeLose(int scoreAdded){
        playerLife = playerLife - 1;
        if(playerLife < 0){
            playerScore = playerScore + (ulong)lifeScoreBonus; 
        }
    }
}