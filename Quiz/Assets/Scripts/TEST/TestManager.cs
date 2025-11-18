using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class Test
{
    public GameObject panelBG;
    public Button choice1;
    public Button choice2;
    public Button choice3;

}
public class TestManager : MonoBehaviour
{
    [Header("ALL Question")]
    public Test[] tests; 
    private char[] answers;
    private bool[] answered;
    private int totalAnswered = 0;
    private bool isTransitioning = false; 

    [Header("UI")]
    public Image fadeImage;

    [Header("Fade Settings")]
    public float fadeDurationIn = 1f;
    public float fadeDurationOut = 1f;

    [Header("Page Story")]
    public GameObject pageStory2;
    public GameObject pageStory3;
    public GameObject pageStory4;
    public GameObject pageStoryCutScene;
    public FlashEffect flashImage;
    private GameObject[] storyPages;


    [Header("Result")]
    public GameObject Result1;
    public GameObject Result2;
    public GameObject Result3;
    public GameObject Result4;
    public GameObject Result5;
    public GameObject Result6;
    public GameObject Result7;
    public GameObject WinResultPanel;

    void Start()
    {
        storyPages = new GameObject[] { pageStory2, pageStory3, pageStory4, pageStoryCutScene };

        foreach (var page in storyPages)
            page.SetActive(false);

        answers = new char[tests.Length];
        answered = new bool[tests.Length];

        for (int i = 0; i < tests.Length; i++)
        {
            int qIndex = i; 

            tests[i].choice1.onClick.AddListener(() => OnAnswerSelected(qIndex, 0)); // A
            tests[i].choice2.onClick.AddListener(() => OnAnswerSelected(qIndex, 1)); // B
            tests[i].choice3.onClick.AddListener(() => OnAnswerSelected(qIndex, 2)); // C
        }

    }

    void OnAnswerSelected(int questionIndex, int choiceIndex)
    {
        if (isTransitioning) return; 

        SoundManager.Instance.PlaySFX("Click");

        char selected = 'A';
        if (choiceIndex == 1) selected = 'B';
        else if (choiceIndex == 2) selected = 'C';

        answers[questionIndex] = selected;

        if (!answered[questionIndex])
        {
            answered[questionIndex] = true;
            totalAnswered++;
        }

        if (totalAnswered >= tests.Length)
        {
            ShowResult();
        }

        isTransitioning = true;
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.DOFade(1f, fadeDurationOut)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                tests[questionIndex].panelBG.SetActive(false);
                switch (questionIndex)
                {
                    case 2:
                        pageStory2.SetActive(true);
                        SoundManager.Instance.PlaySFX("impactful");
                        break;
                    case 4:
                        pageStory3.SetActive(true);
                        SoundManager.Instance.PlayBGM("blocktheme");
                        break;
                    case 5:
                        pageStory4.SetActive(true);
                        break;
                    case 6:
                        pageStoryCutScene.SetActive(true);
                        flashImage.PlayFlash();
                        break;
                    default:
                        if (questionIndex + 1 < tests.Length)
                            tests[questionIndex + 1].panelBG.SetActive(true);
                        break;
                }

                fadeImage.color = Color.black;
                fadeImage.DOFade(0f, fadeDurationIn).SetEase(Ease.InOutSine)
                    .OnComplete(() => { isTransitioning = false; }); 
            });
    }

    void ShowResult()
    {
        int greenEarth = 0;
        int techForward = 0;
        int fullStomach = 0;
        int economicWings = 0;
        int principleStability = 0;
        int globalConnect = 0;
        int softPower = 0;

        for (int i = 0; i < answers.Length; i++)
        {
            char ans = answers[i];

            switch (i)
            {
                case 0: // ข้อ 1
                    if (ans == 'A') fullStomach++;
                    else if (ans == 'B') techForward++;
                    else if (ans == 'C') principleStability++;
                    break;

                case 1: // ข้อ 2
                    if (ans == 'A') softPower++;
                    else if (ans == 'B') economicWings++;
                    else if (ans == 'C') globalConnect++;
                    break;

                case 2: // ข้อ 3
                    if (ans == 'A') fullStomach++;
                    else if (ans == 'B') principleStability++;
                    else if (ans == 'C') greenEarth++;
                    break;

                case 3: // ข้อ 4
                    if (ans == 'A') economicWings++;
                    else if (ans == 'B') techForward++;
                    else if (ans == 'C') softPower++;
                    break;

                case 4: // ข้อ 5
                    if (ans == 'A') greenEarth++;
                    else if (ans == 'B') globalConnect++;
                    else if (ans == 'C') fullStomach++;
                    break;

                case 5: // ข้อ 6
                    if (ans == 'A') principleStability++;
                    else if (ans == 'B') economicWings++;
                    else if (ans == 'C') globalConnect++;
                    break;

                case 6: // ข้อ 7
                    if (ans == 'A') greenEarth++;
                    else if (ans == 'B') techForward++;
                    else if (ans == 'C') softPower++;
                    break;
            }
        }

        int[] scores = {
            greenEarth,
            techForward,
            fullStomach,
            economicWings,
            principleStability,
            globalConnect,
            softPower
        };

        string[] names = {
            "สายกรีนเอิร์ธ",
            "สายเทคฟอร์เวิร์ด",
            "สายปากท้องต้องอิ่ม",
            "สายเศรษฐกิจติดปีก",
            "สายหลักการและความมั่นคง",
            "สายโกลบอลคอนเน็กต์",
            "สายซอฟต์พาวเวอร์"
        };

        int maxIndex = 0;
        for (int i = 1; i < scores.Length; i++)
        {
            if (scores[i] > scores[maxIndex])
                maxIndex = i;
        }
        GameObject[] resultPanels = { Result1, Result2, Result3, Result4, Result5, Result6, Result7 };

        foreach (var panel in resultPanels)
            panel.SetActive(false);

        WinResultPanel = resultPanels[maxIndex];

        Debug.Log("ตัวตนของคุณคือ: " + names[maxIndex]);

    }
}
