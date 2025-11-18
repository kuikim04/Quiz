using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerScripts : MonoBehaviour
{
    public QuizManager QuizManager; 
    public bool isCorrect = false;

    private Image buttonImage;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        if (buttonImage == null)
            Debug.LogWarning("AnswerScripts ต้องอยู่บน GameObject ที่มี Image Component");
    }

    public void Answer()
    {
        if (!QuizManager.Instance.canSelect || MainGameUI.instance.IsGameWin || GameFlowManager.Instance.IsGameMainOver)
            return;

        if (buttonImage == null) return;

        if (isCorrect)
        {
            buttonImage.color = Color.green;
            QuizManager.Instance.Correct();
        }
        else
        {
            buttonImage.color = Color.red;
            QuizManager.Instance.UnCorrect();
        }
    }

}
