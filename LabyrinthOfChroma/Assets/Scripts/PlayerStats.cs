using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("[Stat] Player Settings")]
    [HideInInspector] public GameObject enemyScene;
    [HideInInspector] public GameObject UIScene;
    
    [Header("[Stat] Player Settings")]
    [SerializeField] public ulong playerScore = 0;
    [SerializeField] public ulong highScore = 0;
    [SerializeField] public int enemyKilled = 0;
    [SerializeField] public int beat = 0;
    [SerializeField] public int playerLife;
    [SerializeField] public float playerSpecial = 0.0f;

    [Header("[Stat] Player Stats")]
    [SerializeField] private int numberOrbBreak = 0;
    [SerializeField] public int wave = 0;

    [Header("[Stat] Bonus Stats")]
    [SerializeField] public int keyEnemy = 0;
    [SerializeField] public bool isBonusEnabled = false;
    [SerializeField] private float waveTime = 0f;
    [SerializeField] private float waveTotalTime = 0f;
    [SerializeField] public int waveBonus = 0;
    [SerializeField] public int waveTotalBonus = 0;
    
    
    [Header("[Set] Game Rules")]
    [SerializeField] public int playerMaxLife;
    [SerializeField] public float playerMaxSpecial = 3.0f;
    [SerializeField] private float specialPerOrb;

    void Start()
    {
        highScore = System.Convert.ToUInt64(PlayerPrefs.GetString("HighScore"));
        enemyScene = GameObject.Find("Actor Scene");
        UIScene = GameObject.Find("UI System");
    }

    void Update()
    {
        if(playerScore > highScore)
        {
            PlayerPrefs.SetString("HighScore",""+playerScore);
        }
        
        
        if(playerSpecial > playerMaxSpecial)
        {
            playerSpecial = playerMaxSpecial;
        }
        
        if(isBonusEnabled == true && waveTime >= 0f && waveBonus > 0)
        {
            waveTime = waveTime - Time.deltaTime;
            DecrementBonus();
        }
        else if(waveBonus < 0)
        {
            waveBonus = 0;
        }

        if(isBonusEnabled == true && keyEnemy == 0)
        {
            ConcludeBonus();
        }
    }

    public void KillEnemy()
    {
        enemyKilled++;
        if(enemyKilled % 100 == 0 && wave <= 100)
        {
            LifeAdd();
        }
    }

    public void EnableBonus(float timeLimit, int bonusPoint)
    {
        if(playerLife >= 0)
        {
            isBonusEnabled = true;
            waveTime = timeLimit;
            waveTotalTime = timeLimit;
            waveBonus = bonusPoint;
            waveTotalBonus = bonusPoint;
        }
    }

    public void AddKeyEnemy()
    {
        keyEnemy++;
    }

    public void DestroyKeyEnemy()
    {
        keyEnemy--;
    }

    private void DecrementBonus()
    {
        float bonusLeft = (waveTotalBonus * waveTime) / waveTotalTime;
        
        if(waveBonus <= 0)
        {
            waveBonus = 0;
        }
        else
        {
            waveBonus = (int)Mathf.Floor(bonusLeft);
        }
    }

    private void ConcludeBonus()
    {
        int thisWaveBonus = waveBonus;
        float progression = (float)waveTime / (float)waveTotalTime;
        Debug.Log("Bonus Remaining:"+thisWaveBonus);
        UIScene.GetComponent<GameUI>().BonusEndSignal(thisWaveBonus);

        playerScore = playerScore + (ulong)thisWaveBonus;
        
        isBonusEnabled = false;
        waveTime = 0f;
        waveTotalTime = 0f;
        waveBonus = 0;
        waveTotalBonus = 0;
    }

    public void OrbDestroy(int numberMatched)
    {
        numberOrbBreak = numberOrbBreak + numberMatched;
        playerSpecial = playerSpecial + (numberMatched * specialPerOrb);
    }

    public void ScoreAdd(int scoreAdded)
    { 
        playerScore = playerScore + (ulong)scoreAdded;
        beat = (int)(Mathf.Floor((float)((System.Convert.ToDouble(playerScore)*100)/System.Convert.ToDouble(highScore))));
    }

    public void SpecialAdd(float specialAdded)
    { 
        if(playerSpecial > playerMaxSpecial)
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

    public void NextWave()
    {
        if(playerLife >= 0)
        {
            wave = wave + 1;
        }   
    }


    public void LifeAdd()
    {
        if(playerLife >= playerMaxLife)
        {
            playerScore = playerScore + 1000000;
        }
        else if(playerLife >= 0)
        {
            playerLife = playerLife + 1;
            UIScene.GetComponent<GameUI>().ExtraLifeSignal();
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
            UIScene.GetComponent<GameUI>().DamageScreen();
        }
    }
}