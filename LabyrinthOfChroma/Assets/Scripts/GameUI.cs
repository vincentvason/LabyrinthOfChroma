using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("[Set] Player Settings")]
    [SerializeField] private GameObject playerStat;
    [SerializeField] private GameObject player;

    [Header("[Set] Score Text")]
    [Tooltip("Note: Index 0 is last digit.")]
    [SerializeField] private List<TMP_Text> scoreDigit;

    [Header("[Stat] Score Animation")]
    [SerializeField] private ulong oldScore = 0;
    [SerializeField] private ulong currentScore = 0;
    [SerializeField] private ulong newScore = 0;
    [SerializeField] private ulong scoreIncreaseAnimation = 0;

    [Header("[Set] Beat Text")]
    [Tooltip("Note: Index 0 is last digit.")]
    [SerializeField] private List<TMP_Text> beatDigit;
    [SerializeField] private GameObject beatPanel;
    [SerializeField] private TMP_Text newHighNotification;

    [Header("[Stat] Beat Animation")]
    [SerializeField] private int oldBeat = 0;
    [SerializeField] private int currentBeat = 0;
    [SerializeField] private int newBeat = 0;
    [SerializeField] private int beatIncreaseAnimation = 0;
    [SerializeField] private bool newHighNotificationState = false;
    [SerializeField] private bool newHighAnimationState = false;

    [Header("[Set] Wave Text")]
    [Tooltip("Note: Index 0 is last digit.")]
    [SerializeField] private List<TMP_Text> waveDigit;

    [Header("[Set] Bonus Text")]
    [Tooltip("Note: Index 0 is last digit.")]
    [SerializeField] private List<TMP_Text> bonusDigit;
    [SerializeField] private GameObject bonusPanel;
    [SerializeField] private Slider bonusGauge;
    [SerializeField] private Image bonusGaugeFG;
    [SerializeField] private Image bonusGaugeBG;
    [SerializeField] private GameObject bonusSummaryPanel;
    [SerializeField] private TMP_Text bonusSummaryScore;

    [Header("[Set] Bonus Life Text")]
    [Tooltip("Note: Index 0 is last digit.")]
    [SerializeField] private List<TMP_Text> killDigit;
    [SerializeField] private GameObject bonusLifePanel;

    [Header("[Stat] Bonus Animation")]
    [SerializeField] private bool bonusEndNotificationState = false;
    [SerializeField] private bool bonusEndAnimationState = false;
    [SerializeField] private int bonusEndValue = 0;
    
    [Header("[Set] Life Image")]
    [Tooltip("Note: Index 0 is most left.")]
    [SerializeField] private List<GameObject> lifeIcon;
    [SerializeField] private TMP_Text lastLifeNotification;
    [SerializeField] private TMP_Text gameOverNotification;
    [SerializeField] private TMP_Text extraLifeNotification;
    [SerializeField] private GameObject gameOverScreen;

    [Header("[Stat] Extra Life Animation")]
    [SerializeField] private bool extraLifeNotificationState = false;
    [SerializeField] private bool extraLifeAnimationState = false;

    [Header("[Set] Special Image")]
    [SerializeField] private TMP_Text specialNumber;
    [SerializeField] private Slider specialGauge;
    [SerializeField] private Image specialGaugeFG;
    [SerializeField] private Image specialGaugeBG;
    [SerializeField] private TMP_Text maxSpecialNotification;

    [Header("[Set] Special Animation")]
    [SerializeField] private bool specialNotEnoughState = false;

    [Header("[Set] Damage Animation")]
    [SerializeField] private GameObject damageScreen;

    // Start is called before the first frame update
    void Start()
    {
        playerStat = GameObject.Find("Game System");
        player = GameObject.Find("Player");

        bonusSummaryPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Debug.Log("Escape key was released");
            SceneManager.LoadScene("MainMenu");
        }
        
        if(playerStat.GetComponent<PlayerStats>().playerLife < 0)
        {
            gameOverScreen.SetActive(true);
        }
        
        
        newScore = playerStat.GetComponent<PlayerStats>().playerScore;
        ScoreAnimation();
        ScoreUI(currentScore);
        BonusLifeUI(playerStat.GetComponent<PlayerStats>().enemyKilled % 100);
        LifeUI(playerStat.GetComponent<PlayerStats>().playerLife);
            
        SpecialUI(playerStat.GetComponent<PlayerStats>().playerSpecial, playerStat.GetComponent<PlayerStats>().playerMaxSpecial);

        if(Input.GetMouseButtonDown(2) && playerStat.GetComponent<PlayerStats>().playerSpecial < 1.0f)
        {
            Debug.Log("Special Not Enough");
            StartCoroutine("SpecialNotEnoughFlashCoroutine");
        }

        newBeat = playerStat.GetComponent<PlayerStats>().beat;
        BeatAnimation();
        BeatUI(currentBeat);
        // BeatUI(playerStat.GetComponent<PlayerStats>().beat);
        WaveUI(playerStat.GetComponent<PlayerStats>().wave);
        BonusUI(playerStat.GetComponent<PlayerStats>().isBonusEnabled,playerStat.GetComponent<PlayerStats>().waveBonus,playerStat.GetComponent<PlayerStats>().waveTotalBonus);
    }

    void ScoreAnimation()
    {
        if(currentScore == newScore)
        {
            scoreIncreaseAnimation = 0;
            oldScore = newScore;
        }
        else
        {
            ulong newScoreIncreaseAnimation = (ulong)(((double)newScore - (double)oldScore) * (double)0.01);
            //Debug.Log("score_anim: "+(ulong)(((double)newScore - (double)oldScore) * (double)0.01));
            if(newScoreIncreaseAnimation < (ulong)1)
            {
                scoreIncreaseAnimation = (ulong)1;
            }
            if(newScoreIncreaseAnimation >= scoreIncreaseAnimation)
            {
                scoreIncreaseAnimation = newScoreIncreaseAnimation;
            }
            
        }

        if(currentScore + scoreIncreaseAnimation >= newScore)
        {
            currentScore = newScore;
        }
        else
        {
            currentScore = currentScore + scoreIncreaseAnimation;
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

    void BeatAnimation()
    {
        if(currentBeat == newBeat)
        {
            beatIncreaseAnimation = 0;
            oldBeat = newBeat;
        }
        else
        {
            int newBeatIncreaseAnimation = (int)(((double)newBeat - (double)oldBeat) * (double)0.01);
        //    Debug.Log("beat_anim: "+(int)(((double)newBeat - (double)oldBeat) * (double)0.01));
            if(newBeatIncreaseAnimation < 1)
            {
                beatIncreaseAnimation = 1;
            }
            if(newBeatIncreaseAnimation > beatIncreaseAnimation)
            {
                beatIncreaseAnimation = newBeatIncreaseAnimation;
            }
        }

        if(currentBeat + beatIncreaseAnimation >= newBeat)
        {
            currentBeat = newBeat;
        }
        else
        {
            currentBeat = currentBeat + beatIncreaseAnimation;
        }
    }

    void BeatUI(int beat)
    {
        int totalBeat = beat;

        if(newHighAnimationState == true)
        {
            beatPanel.SetActive(false);
            newHighNotification.enabled = true;
        }
        else if(beat >= 100 && newHighNotificationState == false)
        {
            beatPanel.SetActive(false);
            StartCoroutine("NewHighCoroutine");
            newHighNotificationState = true;
        }
        else if(beat < 100)
        {
            newHighNotification.enabled = false;
            beatPanel.SetActive(true);
            for(int index = 0; index < beatDigit.Count; index++){
                int place = totalBeat % 10;
                beatDigit[index].text = place.ToString();
                if(totalBeat > 0 || index == 0){
                    beatDigit[index].color = Color.yellow;
                }
                else{
                    beatDigit[index].color = Color.gray;
                }
                totalBeat = totalBeat / 10;
            }
        }
    }

    void WaveUI(int wave)
    {
        int totalWave = wave;

        if(wave >= 999)
        {
            wave = 999;
        }
        
        for(int index = 0; index < waveDigit.Count; index++){
            int place = totalWave % 10;
            waveDigit[index].text = place.ToString();
            if(totalWave > 0 || index == 0){
                waveDigit[index].color = Color.white;
            }
            else{
                waveDigit[index].color = Color.gray;
            }
            totalWave = totalWave / 10;
        }
        
    }

    void BonusUI(bool isBonusEnabled, int waveBonus, int waveTotalBonus)
    {
        int totalBonus = waveBonus;
        
        if(bonusEndNotificationState == true)
        {
            Debug.Log("Bonus End from UI");
            bonusPanel.SetActive(false);
            bonusLifePanel.SetActive(false);
            bonusEndAnimationState = true;
            StartCoroutine("BonusEndCoroutine");
            bonusEndNotificationState = false;
        }        
        else if(isBonusEnabled == true)
        {
            bonusPanel.SetActive(true);
            bonusLifePanel.SetActive(false);
            for(int index = 0; index < bonusDigit.Count; index++){
                int place = totalBonus % 10;
                bonusDigit[index].text = place.ToString();
                if(totalBonus > 0 || index == 0){
                    bonusDigit[index].color = Color.magenta;
                }
                else{
                    bonusDigit[index].color = Color.gray;
                }
                totalBonus = totalBonus / 10;
            }
            bonusGauge.value = (float)waveBonus/(float)waveTotalBonus;
        }
        else if(bonusEndAnimationState == false && isBonusEnabled == false)
        {
            bonusPanel.SetActive(false);
            if(playerStat.GetComponent<PlayerStats>().wave < 100)
            {
                bonusLifePanel.SetActive(true);
            }
            else
            {
                bonusLifePanel.SetActive(false);
            }
            
        }
        
    }

    void BonusLifeUI(int kill)
    {
        int totalKill = kill;

        for(int index = 0; index < killDigit.Count; index++){
            int place = totalKill % 10;
            killDigit[index].text = place.ToString();
            if(totalKill > 0 || index == 0){
                killDigit[index].color = new Color(1f, 0.5f, 0.75f, 1f);
            }
            else{
                killDigit[index].color = Color.gray;
            }
            totalKill = totalKill / 10;
        }
    }

    void LifeUI(int life)
    {
        if(extraLifeNotificationState == true)
        {
            StartCoroutine("ExtraLifeCoroutine");
            extraLifeNotificationState = false;
        }
        else if(extraLifeAnimationState == false)
        {
            if(life > 0)
            {
                extraLifeNotification.enabled = false;
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
                extraLifeNotification.enabled = false;
                for(int index = 0; index < lifeIcon.Count; index++){
                    lifeIcon[index].SetActive(false);
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

    public void ExtraLifeSignal()
    {
        extraLifeNotificationState = true;
        extraLifeAnimationState = true;
    }

    private IEnumerator ExtraLifeCoroutine()
    {
        int numberOfFlashes = 15;
        float flashDuration = 0.1f;

        extraLifeNotification.enabled = false;
        lastLifeNotification.enabled = false;
        gameOverNotification.enabled = false;
        for(int index = 0; index < lifeIcon.Count; index++){
            lifeIcon[index].SetActive(false);
        }

        for(int times = 0;times < numberOfFlashes; times++)
        {
            extraLifeNotification.enabled = true;
            yield return new WaitForSeconds(flashDuration);
            extraLifeNotification.enabled = false;
            yield return new WaitForSeconds(flashDuration);
        }
        extraLifeAnimationState = false;
        yield return null;
    }

    public void BonusEndSignal(int bonus)
    {
        bonusEndNotificationState = true;
        bonusEndValue = bonus;
    }

    private IEnumerator BonusEndCoroutine()
    {
        int numberOfFlashes = 15;
        float flashDuration = 0.1f;
        
        bonusSummaryScore.text = "+" + bonusEndValue;

        for(int times = 0;times < numberOfFlashes; times++)
        {
            bonusSummaryPanel.SetActive(true);
            yield return new WaitForSeconds(flashDuration);
            bonusSummaryPanel.SetActive(false);
            yield return new WaitForSeconds(flashDuration);
        }
        bonusEndAnimationState = false;
        yield return null;
    }

    
    private IEnumerator NewHighCoroutine()
    {
        int numberOfFlashes = 15;
        float flashDuration = 0.1f;

        for(int times = 0;times < numberOfFlashes; times++)
        {
            newHighNotification.enabled = false;
            yield return new WaitForSeconds(flashDuration);
            newHighNotification.enabled = true;
            yield return new WaitForSeconds(flashDuration);
        }
        newHighAnimationState = true;
        yield return null;
    }

    public void DamageScreen()
    {
        StartCoroutine("DamageCoroutine");
    }

    private IEnumerator DamageCoroutine()
    {
        int numberOfFlashes = 5;
        float flashDuration = 0.02f;

        for(int times = 0;times < numberOfFlashes; times++)
        {
            damageScreen.SetActive(true);
            yield return new WaitForSeconds(flashDuration);
            damageScreen.SetActive(false);
            yield return new WaitForSeconds(flashDuration);
        }
        yield return null;
    }

}
