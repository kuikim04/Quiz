using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TestUI : MonoBehaviour
{
    [Header("Player info")]
    private string playerName;
    public InputField InputFieldName;
    public Button setNameButton;
    public TextMeshProUGUI textName;

    [Header("Checklist")]
    public Toggle checklist1;
    public Toggle checklist2;
    public Toggle checklist3;
    public Button nextButton;

    [Header("UI")]
    public Image fadeImage;

    [Header("Fade Settings")]
    public float fadeDurationIn = 1f;
    public float fadeDurationOut = 1f;

    [Header("PAGE")]
    public GameObject page1;
    public GameObject page2;
    public GameObject page3;
    public GameObject page4;
    public GameObject page5;
    public float delayStart = 5f;
    public float delayNormal = 1f;


    [Header("Story")]
    public GameObject Story1;
    public GameObject NextStory1;
    public GameObject Story2;
    public GameObject NextStory2;
    public GameObject Story3;
    public GameObject NextStory3;
    public GameObject Story4;
    public GameObject NextStory4;
    public GameObject Story5;
    public GameObject NextStory5;

    [Header("Result")]
    public TestManager testManager;

    void Start()
    {
        SoundManager.Instance.PlayBGM("clovers");

        page1.SetActive(true);
        page2.SetActive(false);

        if (fadeImage != null)
        {
            fadeImage.color = Color.black;
            fadeImage.DOFade(0f, fadeDurationIn)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    Invoke(nameof(ShowNextPage), delayStart);
                });
        }

        checklist1.onValueChanged.AddListener(delegate { CheckChecklist(); });
        checklist2.onValueChanged.AddListener(delegate { CheckChecklist(); });
        checklist3.onValueChanged.AddListener(delegate { CheckChecklist(); });

    }
    void ShowNextPage()
    {
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.DOFade(1f, fadeDurationOut)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                page1.SetActive(false);

                page2.SetActive(true);
                fadeImage.color = Color.black;
                fadeImage.DOFade(0f, fadeDurationIn).SetEase(Ease.InOutSine);
            });
    }
    public void OnInputChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            SoundManager.Instance.PlaySFX("type");
            setNameButton.interactable = true;
        }
        else
        {
            setNameButton.interactable = false;
        }
    }
    public void SetName()
    {
        SoundManager.Instance.PlaySFX("Click");
        
        string name = "";
        name = InputFieldName.text;
        playerName = name;
        textName.text = $"อรุณสวัสดิ์ครับ สส.{playerName}\r\nพร้อมไปประชุมหรือยังครับ";
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.DOFade(1f, fadeDurationOut)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                page2.SetActive(false);

                page3.SetActive(true);
                fadeImage.color = Color.black;
                fadeImage.DOFade(0f, fadeDurationIn).SetEase(Ease.InOutSine);
            });
    }
    public void NextPage()
    {
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.DOFade(1f, fadeDurationOut)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                page3.SetActive(false);
                page4.SetActive(true);

                fadeImage.color = Color.black;
                fadeImage.DOFade(0f, fadeDurationIn).SetEase(Ease.InOutSine);
            });
    }
    void CheckChecklist()
    {
        SoundManager.Instance.PlaySFX("Pen");

        if (checklist1.isOn && checklist2.isOn && checklist3.isOn)
        {
            nextButton.interactable = true;
        }
        else
        {
            nextButton.interactable = false;
        }
    }
    public void ReadyCheckStart()
    {
        SoundManager.Instance.PlaySFX("Click");

        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.DOFade(1f, fadeDurationOut)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                page4.SetActive(false);
                page5.SetActive(true);

                SoundManager.Instance.PlayBGM("8bitcanon");

                fadeImage.color = Color.black;
                fadeImage.DOFade(0f, fadeDurationIn).SetEase(Ease.InOutSine);
            });
    }

    public void NextStoryPath1()
    {
        SoundManager.Instance.PlaySFX("Click");

        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.DOFade(1f, fadeDurationOut)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                Story1.SetActive(false);
                NextStory1.SetActive(true);

                fadeImage.color = Color.black;
                fadeImage.DOFade(0f, fadeDurationIn).SetEase(Ease.InOutSine);
            });
    }
    public void NextStoryPath2()
    {
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.DOFade(1f, fadeDurationOut)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                Story2.SetActive(false);
                NextStory2.SetActive(true);

                fadeImage.color = Color.black;
                fadeImage.DOFade(0f, fadeDurationIn).SetEase(Ease.InOutSine);
            });
    }
    public void NextStoryPath3()
    {
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.DOFade(1f, fadeDurationOut)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                Story3.SetActive(false);
                NextStory3.SetActive(true);

                fadeImage.color = Color.black;
                fadeImage.DOFade(0f, fadeDurationIn).SetEase(Ease.InOutSine);
            });
    }
    public void NextStoryPath4()
    {
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.DOFade(1f, fadeDurationOut)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                Story4.SetActive(false);
                NextStory4.SetActive(true);

                fadeImage.color = Color.black;
                fadeImage.DOFade(0f, fadeDurationIn).SetEase(Ease.InOutSine);
            });
    }
    public void NextStoryPath5()
    {
        NextStory5 = testManager.WinResultPanel;

        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.DOFade(1f, fadeDurationOut)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                Story5.SetActive(false);
                NextStory5.SetActive(true);
                fadeImage.color = Color.black;
                fadeImage.DOFade(0f, fadeDurationIn).SetEase(Ease.InOutSine);

                SoundManager.Instance.StopBGM();
                SoundManager.Instance.PlaySFX("win");

            });
    }

    public void BackToMainMenu()
    {
        FadeOutAndLoad("MainGamePlay");
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
