using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialControllerMain : MonoBehaviour
{
    [Header("Tutorial Pages")]
    public GameObject tutorialPanel;
    public RectTransform tutorial1;
    private float animDuration = 0.4f;

    private void Start()
    {
        if (!GameFlowManager.Instance.IsOpenFirst)
            return;

        SoundManager.Instance.StopBGM();

        tutorialPanel.SetActive(true);
        tutorial1.gameObject.SetActive(true);

        tutorial1.anchoredPosition = Vector2.zero;

    }

    // -----------------------------
    // ปุ่มกดหน้าแรก
    // -----------------------------
    public void ClosePage1()
    {
        SoundManager.Instance.PlaySFX("Click");

        tutorial1.DOAnchorPos(new Vector2(-2000f, 0f), animDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                tutorial1.gameObject.SetActive(false);
                tutorialPanel.SetActive(false);

                SoundManager.Instance.PlayBGM("pixela");
                QuizManager.Instance.isStart = true;
                GameFlowManager.Instance.IsOpenFirst = false;
            });
    }
}
