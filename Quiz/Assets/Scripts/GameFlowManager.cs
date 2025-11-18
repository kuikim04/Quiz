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
    private QuizAndAnswer lastQuestion = null;

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
        questionsSinceMinigame++;
        totalAnswered++;

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
            LoadRandomMiniGame();
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
        lastQuestion = remainingQuestions[index]; // เก็บคำถามปัจจุบัน
        return lastQuestion; // **ยังไม่ลบ**
    }

    public void RemoveCurrentQuestion()
    {
        if (lastQuestion != null)
        {
            remainingQuestions.Remove(lastQuestion);
            lastQuestion = null;
        }
    }


    void LoadRandomMiniGame()
    {
        string[] minigames = { "Minigame1", "Minigame2", "Minigame3" };
        string selected = minigames[Random.Range(0, minigames.Length)];
        SceneManager.LoadScene(selected);
    }

    public void GameOver()
    {
        MainGameUI.instance.GameOver();
    }
}
