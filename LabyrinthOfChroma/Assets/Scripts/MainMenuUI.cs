using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("[Set] High Score")]
    [SerializeField] private ulong highScore;

    [Header("[Set] High Score Text")]
    [Tooltip("Note: Index 0 is last digit.")]
    [SerializeField] private List<TMP_Text> scoreDigit;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("HighScore") == false)
        {
            PlayerPrefs.SetString("HighScore","10000");
        }
        else if(System.Convert.ToUInt64(PlayerPrefs.GetString("HighScore")) < 10000)
        {
            PlayerPrefs.SetString("HighScore","10000");
        }

        highScore = System.Convert.ToUInt64(PlayerPrefs.GetString("HighScore"));
        
        HighScoreUI(highScore);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HighScoreUI(ulong score)
    {
        ulong totalScore = score;

        for(int index = 0; index < scoreDigit.Count; index++){
            ulong place = totalScore % (ulong)10;
            scoreDigit[index].text = place.ToString();
            if(totalScore > 0 || index == 0){
                scoreDigit[index].color = Color.cyan;
            }
            else{
                scoreDigit[index].color = Color.gray;
            }
            totalScore = totalScore / (ulong)10;
        }
    }
}
