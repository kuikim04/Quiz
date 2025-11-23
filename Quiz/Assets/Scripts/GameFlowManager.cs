using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance;

    [Header("Quiz Data")]
    public List<QuizAndAnswer> allQuestions;
    private List<QuizAndAnswer> remainingQuestions; 

    [HideInInspector] public int questionsSinceMinigame = 0;
    [HideInInspector] public int totalAnswered = 0;

    [HideInInspector] public int correctCount = 0;
    [HideInInspector] public int wrongCount = 0;

    public void AddCorrect() => correctCount++;
    public void AddWrong() => wrongCount++;

    public bool IsGameMainOver = false;
    public bool IsOpenFirst = true;
    private QuizAndAnswer lastQuestion = null;

    int nextMiniGameIndex = 0;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        IsOpenFirst = true;
        ResetQuiz();
    }

    public void ResetQuiz()
    {
        if (allQuestions == null || allQuestions.Count == 0)
        {
            Debug.LogError("ยังไม่ได้กำหนด allQuestions ใน GameFlowManager!");
            return;
        }

        remainingQuestions = new List<QuizAndAnswer>(allQuestions);
        questionsSinceMinigame = 0;
        totalAnswered = 0;
        correctCount = 0;
        wrongCount = 0;
    }

    public void QuestionAnswered()
    {
        if (totalAnswered >= 10 || remainingQuestions.Count == 0)
        {
            MainGameUI.instance.PlayWinAnimation();
            Debug.Log("Finish");

            return;
        }

        if (questionsSinceMinigame >= 2)
        {
            if (IsGameMainOver)
                return;

            questionsSinceMinigame = 0;
            LoadMiniGameSequential();
        }
        else
        {
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene != "SceneGamePlay")
            {
                SceneManager.LoadScene("SceneGamePlay");
            }
            else
            {
                if (IsGameMainOver)
                    return;

                QuizManager.Instance.GenerateQuestion();
                Debug.Log("Already in SceneGamePlay → ไม่โหลดซ้ำ");
            }
        }
    }

    public QuizAndAnswer GetNextQuestion()
    {
        if (remainingQuestions == null || remainingQuestions.Count == 0)
        {
            ResetQuiz();
        }

        if (remainingQuestions.Count == 0) return null;

        int index = Random.Range(0, remainingQuestions.Count);
        lastQuestion = remainingQuestions[index]; 
        return lastQuestion; 
    }

    public void RemoveCurrentQuestion()
    {
        if (lastQuestion != null)
        {
            remainingQuestions.Remove(lastQuestion);
            lastQuestion = null;
        }
    }


    void LoadMiniGameSequential()
    {
        string[] minigames = { "Minigame1", "Minigame2", "Minigame3", "Minigame4"};

        nextMiniGameIndex = nextMiniGameIndex % minigames.Length;

        string selected = minigames[nextMiniGameIndex];
        nextMiniGameIndex++;

        SceneManager.LoadScene(selected);
    }


    public void GameOver()
    {
        IsGameMainOver  = true;
        MainGameUI.instance.GameOver();
    }
}
