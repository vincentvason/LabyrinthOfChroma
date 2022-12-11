using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("[Set] Player Settings")]
    [SerializeField] private PlayerStats playerStat;
    [SerializeField] private GameObject player;

    [Header("[Set] Score Text")]
    [Tooltip("Note: Index 0 is last digit.")]
    [SerializeField] private List<TMP_Text> scoreDigit;

    [Header("[Set] Life Image")]
    [Tooltip("Note: Index 0 is most left.")]
    [SerializeField] private List<GameObject> lifeIcon;
    [SerializeField] private TMP_Text lastLifeNotification;
    [SerializeField] private TMP_Text gameOverNotification;

    [Header("[Set] Special Image")]
    [SerializeField] private TMP_Text specialNumber;
    [SerializeField] private bool specialNotEnoughState = false;
    [SerializeField] private Slider specialGauge;
    [SerializeField] private Image specialGaugeFG;
    [SerializeField] private Image specialGaugeBG;
    [SerializeField] private TMP_Text maxSpecialNotification;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ScoreUI(playerStat.GetComponent<PlayerStats>().playerScore);
        LifeUI(playerStat.GetComponent<PlayerStats>().playerLife);
        SpecialUI(playerStat.GetComponent<PlayerStats>().playerSpecial, playerStat.GetComponent<PlayerStats>().playerMaxSpecial);

        if(Input.GetMouseButtonDown(2) && playerStat.GetComponent<PlayerStats>().playerSpecial < 1.0f)
        {
            Debug.Log("Special Not Enough");
            StartCoroutine("SpecialNotEnoughFlashCoroutine");
        }
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
                if(life >= index+1){
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

    void SpecialUI(float special, float max)
    {
        specialNumber.text = ((int)special).ToString();

        if(special >= max)
        {
            specialGauge.value = 1;
            maxSpecialNotification.enabled = true;
        }
        else
        {
            specialGauge.value = special % 1.0f;
            maxSpecialNotification.enabled = false;
        }

        if(player.transform.position.x < -2.0f && player.transform.position.y < -4.0f)
        {
            if(specialNotEnoughState == false)
            {
                specialNumber.color = new Color32(255,255,255,64);
            }
            maxSpecialNotification.color = new Color32(255,255,255,64);
            specialGaugeFG.color = new Color32(255,0,0,64);
            specialGaugeBG.color = new Color32(0,0,0,32);
        }
        else
        {
            if(specialNotEnoughState == false)
            {
                specialNumber.color = new Color32(255,255,255,255);
            }
            maxSpecialNotification.color = new Color32(255,255,255,255);
            specialGaugeFG.color = new Color32(255,0,0,255);
            specialGaugeBG.color = new Color32(0,0,0,128);
        }

    }

    private IEnumerator SpecialNotEnoughFlashCoroutine()
    {
        int numberOfFlashes = 10;
        float flashDuration = 0.05f;

        specialNotEnoughState = true;
        for(int times = 0;times < numberOfFlashes; times++)
        {
            if(player.transform.position.x < -2.0f && player.transform.position.y < -4.0f)
            {
                specialNumber.color = new Color32(255,0,0,64);
            }
            else
            {
                specialNumber.color = new Color32(255,0,0,255); 
            }
            yield return new WaitForSeconds(flashDuration);
            if(player.transform.position.x < -2.0f && player.transform.position.y < -4.0f)
            {
                specialNumber.color = new Color32(255,255,255,64);
            }
            else
            {
                specialNumber.color = new Color32(255,255,255,255); 
            }
            yield return new WaitForSeconds(flashDuration);
        }
        specialNotEnoughState = false;

        yield return null;
    }
}
