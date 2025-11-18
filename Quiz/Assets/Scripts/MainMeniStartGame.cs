using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMeniStartGame : MonoBehaviour
{
    public GameObject Start_Page;
    public GameObject SelectMode_Page;

    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeDurationIn = 1f;  
    public float fadeDurationOut = 1f; 

    void Start()
    {
        SoundManager.Instance.PlayBGM("pixel-party");
        
        if (fadeImage != null)
        {
            fadeImage.color = Color.black;
            fadeImage.DOFade(0f, fadeDurationIn).SetEase(Ease.InOutSine);
        }

        var gfm = GameFlowManager.Instance;
        var gfn = HeartManager.Instance;

        if (gfm != null)
        {
            Destroy(gfm.gameObject);
        }
        if (gfn != null)
        {
            Destroy(gfn.gameObject);
        }
    }

    public void StartGame()
    {
        SoundManager.Instance.PlaySFX("Click");

        Start_Page.SetActive(false);
        SelectMode_Page.SetActive(true);
    }

    public void QuizMode()
    {
        SoundManager.Instance.PlaySFX("Click");
        SoundManager.Instance.StopBGM();
        FadeOutAndLoad("SceneGamePlay");
    }

    public void TestMode()
    {
        SoundManager.Instance.PlaySFX("Click");
        SoundManager.Instance.StopBGM();
        FadeOutAndLoad("PotikaScene");
    }


    void FadeOutAndLoad(string sceneName)
    {
        fadeImage.color = new Color(0, 0, 0, 0);    
        fadeImage.DOFade(1f, fadeDurationOut)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                SceneManager.LoadScene(sceneName);
            });
    }
}
