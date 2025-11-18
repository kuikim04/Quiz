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

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetupHearts();
        GenerateQuestion();
    }

    public void GenerateQuestion()
    {
        canSelect = true;

        currentQuestion = GameFlowManager.Instance.GetNextQuestion();

        if (currentQuestion == null)
        {
            Debug.Log("Finish");
            return;
        }

        QuestionText.text = currentQuestion.Question;

        int currentIndex = GameFlowManager.Instance.totalAnswered + 1;
        ReminingQuestionText.text = $"Question {currentIndex}/10";

        List<int> indices = new List<int> { 0, 1, 2, 3 }; 
                                                          
        for (int i = 0; i < indices.Count; i++)
        {
            int rnd = Random.Range(i, indices.Count);
            int temp = indices[i];
            indices[i] = indices[rnd];
            indices[rnd] = temp;
        }

        int correctIndexInOptions = indices[0]; 

        for (int i = 0; i < options.Length; i++)
        {
            AnswerScripts answerScript = options[i].GetComponent<AnswerScripts>();
            answerScript.isCorrect = (i == correctIndexInOptions);

            int answerDataIndex = i;
            if (i == correctIndexInOptions)
                answerDataIndex = currentQuestion.CorrectAnswer - 1; 
            else
            {
                List<int> otherIndices = new List<int> { 0, 1, 2, 3 };
                otherIndices.Remove(currentQuestion.CorrectAnswer - 1);
                answerDataIndex = otherIndices[Random.Range(0, otherIndices.Count)];
            }

            options[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text
                = currentQuestion.Answers[answerDataIndex];

            options[i].GetComponent<UnityEngine.UI.Image>().color = Color.white; 
            options[i].GetComponent<UnityEngine.UI.Button>().interactable = true;

        }
    }

    public void Correct()
    {
        if (!canSelect) return;
        canSelect = false;

        DisableAllOptions();

        GameFlowManager.Instance.AddCorrect();
        GameFlowManager.Instance.RemoveCurrentQuestion();

        StartCoroutine(ShowAnswerAndNext());
    }

    public void UnCorrect()
    {
        if (!canSelect) return;
        canSelect = false;

        DisableAllOptions();

        GameFlowManager.Instance.AddWrong();
        RemoveHeart();
        StartCoroutine(ShowAnswerAndNext());
    }

    private void DisableAllOptions()
    {
        foreach (var option in options)
            option.GetComponent<UnityEngine.UI.Button>().interactable = false;
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
