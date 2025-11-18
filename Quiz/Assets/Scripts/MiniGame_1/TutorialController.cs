using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [Header("Tutorial Pages")]
    public GameObject tutorialPanel;
    public RectTransform tutorial1;
    public RectTransform tutorial2;

    [Header("Cooldown UI")]
    public TextMeshProUGUI cooldownText;

    private float animDuration = 0.4f;

    private void Start()
    {
        SoundManager.Instance.StopBGM();

        tutorialPanel.SetActive(true);
        tutorial1.gameObject.SetActive(true);
        tutorial2.gameObject.SetActive(false);
        cooldownText.gameObject.SetActive(false);

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

                tutorial2.gameObject.SetActive(true);
                tutorial2.anchoredPosition = new Vector2(2000f, 0f);

                tutorial2.DOAnchorPos(Vector2.zero, animDuration)
                    .SetEase(Ease.OutBack);
            });
    }

    // -----------------------------
    // ปุ่มกดหน้าที่สอง
    // -----------------------------
    public void ClosePage2AndStartCooldown()
    {
        SoundManager.Instance.PlaySFX("Click");

        tutorial2.DOAnchorPos(new Vector2(-2000f, 0f), animDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                tutorial2.gameObject.SetActive(false); 
                tutorialPanel.SetActive(false);

                StartCoroutine(StartCooldown());
            });
    }

    // -----------------------------
    // ระบบนับถอยหลัง 3 วิ → START → เริ่มเกม
    // -----------------------------
    IEnumerator StartCooldown()
    {
        cooldownText.gameObject.SetActive(true);
        SoundManager.Instance.PlaySFX("Count");

        int timeLeft = 3;

        while (timeLeft > 0)
        {
            cooldownText.text = timeLeft.ToString();
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        cooldownText.text = "START!";

        yield return new WaitForSeconds(0.5f);

        cooldownText.gameObject.SetActive(false);

        GameManagerMiniGame1.Instance.StartGame();

        SoundManager.Instance.PlayBGM("Mini1Sound");
    }
}
