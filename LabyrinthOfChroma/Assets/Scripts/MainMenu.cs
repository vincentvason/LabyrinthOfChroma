using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    public void LoadMainMenuScene()
    {
        DOTween.KillAll();
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadMainGameScene()
    {
        DOTween.KillAll();
        SceneManager.LoadScene("MainGame");
    }

    public void LoadHowToPlayScene()
    {
        DOTween.KillAll();
        SceneManager.LoadScene("HowToPlay");
    }
}
