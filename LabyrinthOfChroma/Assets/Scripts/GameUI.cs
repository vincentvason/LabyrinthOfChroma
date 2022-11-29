using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("[Set] Player Settings")]
    [SerializeField] private GameObject player;

    [Header("[Set] Score Text")]
    [Tooltip("Note: Index 0 is last digit.")]
    [SerializeField] private List<TMP_Text> scoreDigit;

    [Header("[Set] Life Image")]
    [Tooltip("Note: Index 0 is most left.")]
    [SerializeField] private List<GameObject> lifeIcon;
    [SerializeField] private TMP_Text lastLifeNotification;
    [SerializeField] private TMP_Text gameOverNotification;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ScoreUI(player.GetComponent<PlayerStats>().playerScore);
        LifeUI(player.GetComponent<PlayerStats>().playerLife);
    }

    void ScoreUI(ulong score)
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

    void LifeUI(int life)
    {
        if(life > 0)
        {
            lastLifeNotification.enabled = false;
            gameOverNotification.enabled = false;
            for(int index = 0; index < lifeIcon.Count; index++){
                lifeIcon[index].SetActive(true);
                if(player.GetComponent<PlayerStats>().playerLife >= index+1){
                    lifeIcon[index].GetComponent<Image>().color = Color.red;
                }
                else{
                    lifeIcon[index].GetComponent<Image>().color = Color.gray;
                }
            }
        }
        else
        {
            for(int index = 0; index < lifeIcon.Count; index++){
                lifeIcon[index].SetActive(false);
                lastLifeNotification.enabled = false;
                gameOverNotification.enabled = false;
            }
            if(life == 0){
                lastLifeNotification.enabled = true;
                gameOverNotification.enabled = false;
            }
            else{
                lastLifeNotification.enabled = false;
                gameOverNotification.enabled = true;
            }
        }
    }
}
