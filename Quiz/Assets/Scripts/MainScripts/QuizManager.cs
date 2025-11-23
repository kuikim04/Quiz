using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    public static QuizManager Instance; 
    public GameObject[] options; 
    public TextMeshProUGUI QuestionText;
    public TextMeshProUGUI ReminingQuestionText;

    public Transform heartsContainer;
    public GameObject heartPrefab;
    public List<GameObject> hearts = new();

    private QuizAndAnswer currentQuestion;
    public bool canSelect = true;

    public TextMeshProUGUI timerText;
    private float timeLeft = 30f;
    public bool timerRunning = false;
    public bool isStart = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetupHearts();
        GenerateQuestion();
    }
    void Update()
    {
        if (!isStart) return;
        if (!timerRunning) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft < 0)
            timeLeft = 0;

        timerText.text = $"Time: {Mathf.Ceil(timeLeft)}s";

        if (timeLeft <= 0)
        {
            timerRunning = false;
            UnCorrectTimeup();
        }
    }

    public void GenerateQuestion()
    {
        canSelect = true;

        currentQuestion = GameFlowManager.Instance.GetNextQuestion();
        if (currentQuestion == null) return;

        QuestionText.text = currentQuestion.Question;

        int currentIndex = GameFlowManager.Instance.totalAnswered;
        ReminingQuestionText.text = $"Correct Answers: {currentIndex}/10";

        string correctAns = currentQuestion.Answers[currentQuestion.CorrectAnswer - 1];

        List<string> wrongAns = new List<string>();
        for (int i = 0; i < currentQuestion.Answers.Length; i++)
        {
            if (i != currentQuestion.CorrectAnswer - 1)
                wrongAns.Add(currentQuestion.Answers[i]);
        }

        List<string> answerPool = new List<string>();

        answerPool.Add(correctAns);
        answerPool.AddRange(wrongAns);

        for (int i = 0; i < answerPool.Count; i++)
        {
            int rnd = Random.Range(i, answerPool.Count);
            (answerPool[i], answerPool[rnd]) = (answerPool[rnd], answerPool[i]);
        }

        for (int i = 0; i < options.Length; i++)
        {
            string ans = answerPool[i];

            options[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ans;

            bool isCorrect = ans == correctAns;
            options[i].GetComponent<AnswerScripts>().isCorrect = isCorrect;

            options[i].GetComponent<UnityEngine.UI.Button>().interactable = true;
            options[i].GetComponent<UnityEngine.UI.Image>().color = Color.white;
        }

        timeLeft = 30f;
        timerRunning = true;
    }


    public void Correct()
    {
        if (!canSelect) return;
        canSelect = false;
        timerRunning = false;

        GameFlowManager.Instance.totalAnswered++;
        GameFlowManager.Instance.questionsSinceMinigame++;

        DisableAllOptions();

        GameFlowManager.Instance.AddCorrect();
        GameFlowManager.Instance.RemoveCurrentQuestion();
        StartCoroutine(ShowAnswerAndNext());
    }

    public void UnCorrect()
    {
        if (!canSelect) return;
        canSelect = false;
        timerRunning = false;  

        DisableAllOptions();
        ShowCorrectAnswer();
        GameFlowManager.Instance.AddWrong();

        RemoveHeart();
        StartCoroutine(ShowAnswerAndNext());
    }
    public void UnCorrectTimeup()
    {
        if (!canSelect) return;
        canSelect = false;
        timerRunning = false;

        DisableAllOptions(); foreach (var option in options)
        {
            var btn = option.GetComponent<UnityEngine.UI.Button>();
            if (!option.GetComponent<AnswerScripts>().isCorrect)
            {
                if (!btn.interactable)
                    option.GetComponent<UnityEngine.UI.Image>().color = Color.red;
            }
        }

        ShowCorrectAnswer();
        GameFlowManager.Instance.AddWrong();

        RemoveHeart();
        StartCoroutine(ShowAnswerAndNext());
    }


    private void DisableAllOptions()
    {
        foreach (var option in options)
            option.GetComponent<UnityEngine.UI.Button>().interactable = false;
    }

    private void ShowCorrectAnswer()
    {
        foreach (var option in options)
        {
            var img = option.GetComponent<UnityEngine.UI.Image>();
            var ans = option.GetComponent<AnswerScripts>();

            if (ans.isCorrect)
                img.color = Color.green;
        }
    }

    void SetupHearts()
    {
        foreach (Transform child in heartsContainer)
            Destroy(child.gameObject);

        hearts.Clear();

        for (int i = 0; i < HeartManager.Instance.maxHearts; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsContainer);
            hearts.Add(heart);
        }

        UpdateHeartsUI();
    }
    void UpdateHeartsUI()
    {
        int heartsLeft = HeartManager.Instance.currentHearts;

        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].SetActive(i < heartsLeft);
        }
    }

    void RemoveHeart()
    {
        if (!HeartManager.Instance.LoseHeart())
            return;

        GameObject targetHeart = null;

        for (int i = hearts.Count - 1; i >= 0; i--)
        {
            if (hearts[i].activeSelf)    
            {
                targetHeart = hearts[i];
                break;
            }
        }

        if (targetHeart == null) return;

        targetHeart.transform.DOScale(0, 0.25f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                targetHeart.SetActive(false);
                targetHeart.transform.localScale = Vector3.one; // ÃÕà«çµ scale
                UpdateHeartsUI();
            });
    }

    private IEnumerator ShowAnswerAndNext()
    {
        yield return new WaitForSecondsRealtime(1f);

        GameFlowManager.Instance.QuestionAnswered();
    }
}
