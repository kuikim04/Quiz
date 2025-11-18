using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerMiniGame1 : MonoBehaviour
{
    public static GameManagerMiniGame1 Instance;

    [Header("Progress Bar")]
    public Image progressBar;
    public Image movingImage;
    public float duration = 60f;
    private bool isCompleted = false;

    [Header("Heart UI")]
    public Transform heartsContainer;
    public GameObject heartPrefab;
    public List<GameObject> hearts = new();

    public GameObject gameOverPanel;
    public GameObject gameWinPanel;

    public bool isGameStart = false;
    public bool isGameOver = false;
    public bool isGameWin = false;

    [Header("UI")]
    public Image fadeImage;

    [Header("Fade Settings")]
    public float fadeDurationIn = 1f;
    public float fadeDurationOut = 1f;

    // ========================================
    // KNOWLEDGE SYSTEM 
    // ========================================
    [Header("Knowledge Panels (10 panels)")]
    public List<GameObject> knowledgePanels = new();

    [Header("Buttons")]
    public Button watchInfoButton;
    public Button knowledgeContinueButton;

    [Header("Cooldown Image")]
    public Image cooldownImage;

    private GameObject currentKnowledgePanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Fade-in
        if (fadeImage != null)
        {
            fadeImage.color = Color.black;
            fadeImage.DOFade(0f, fadeDurationIn)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => fadeImage.gameObject.SetActive(false));
        }

        // Hearts
        SetupHeartsUI();
        UpdateHeartsUI();

        // Progress
        if (progressBar != null)
            progressBar.fillAmount = 0f;

        // Knowledge System Setup
        InitKnowledgeSystem();
    }

    // ========================================
    #region Knowledge System
    void InitKnowledgeSystem()
    {
        foreach (var p in knowledgePanels)
            p.SetActive(false);

        cooldownImage.fillAmount = 0;
        cooldownImage.gameObject.SetActive(false);

        knowledgeContinueButton.gameObject.SetActive(false);
        knowledgeContinueButton.interactable = false;

        if (watchInfoButton != null)
            watchInfoButton.onClick.AddListener(OpenRandomKnowledge);

        if (knowledgeContinueButton != null)
            knowledgeContinueButton.onClick.AddListener(OnContinue);
    }

    private void OpenRandomKnowledge()
    {
        SoundManager.Instance.PlaySFX("Click");

        watchInfoButton.interactable = false;

        int index = Random.Range(0, knowledgePanels.Count);
        currentKnowledgePanel = knowledgePanels[index];

        currentKnowledgePanel.SetActive(true);
        knowledgeContinueButton.gameObject.SetActive(true);

        StartCoroutine(StartCooldown());
    }

    IEnumerator StartCooldown()
    {
        cooldownImage.gameObject.SetActive(true);
        cooldownImage.fillAmount = 1f;

        float cd = 5f;

        cooldownImage.DOFillAmount(0f, cd).SetEase(Ease.Linear);

        knowledgeContinueButton.interactable = false;

        yield return new WaitForSecondsRealtime(cd);

        cooldownImage.gameObject.SetActive(false);
        knowledgeContinueButton.interactable = true;
    }

    private void OnContinue()
    {
        SoundManager.Instance.PlaySFX("Click");

        // รีเซ็ตหัวใจ 1 ดวง
        HeartManager.Instance.currentHearts++;
        if (HeartManager.Instance.currentHearts > HeartManager.Instance.maxHearts)
            HeartManager.Instance.currentHearts = HeartManager.Instance.maxHearts;

        UpdateHeartsUI();

        knowledgeContinueButton.interactable = false;

        FadeOutAndLoadScene("SceneGamePlay");
    }
    public void BackMain()
    {
        SoundManager.Instance.PlaySFX("Click");
        FadeOutAndLoadScene("MainGamePlay"); 
    }

    private void FadeOutAndLoadScene(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(0, 0, 0, 0);

        fadeImage.DOFade(1f, fadeDurationOut)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                SceneManager.LoadScene(sceneName);
            });
    }

    #endregion
    // ========================================

    public void StartGame()
    {
        if (isGameStart) return;

        isGameStart = true;

        if (progressBar != null)
            StartCoroutine(FillProgressBar());
    }

    // ========================================
    #region Hearts
    void SetupHeartsUI()
    {
        foreach (Transform child in heartsContainer)
            Destroy(child.gameObject);

        hearts.Clear();

        for (int i = 0; i < HeartManager.Instance.maxHearts; i++)
        {
            GameObject h = Instantiate(heartPrefab, heartsContainer);
            hearts.Add(h);
        }
    }

    void UpdateHeartsUI()
    {
        int heartsLeft = HeartManager.Instance.currentHearts;

        for (int i = 0; i < hearts.Count; i++)
            hearts[i].SetActive(i < heartsLeft);
    }

    public void LoseHeart()
    {
        if (!isGameStart) return;

        if (!HeartManager.Instance.LoseHeart())
            return;

        RemoveHeartAnimation();

        if (HeartManager.Instance.currentHearts <= 0)
            GameOver();
    }

    void RemoveHeartAnimation()
    {
        GameObject target = null;

        for (int i = hearts.Count - 1; i >= 0; i--)
        {
            if (hearts[i].activeSelf)
            {
                target = hearts[i];
                break;
            }
        }

        if (target == null) return;

        target.transform.DOScale(0, 0.25f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                target.SetActive(false);
                target.transform.localScale = Vector3.one;
                UpdateHeartsUI();
            });
    }
    #endregion
    // ========================================

    #region ProgressBar
    IEnumerator FillProgressBar()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (!isGameStart) { yield return null; continue; }

            if (isCompleted || isGameOver) yield break;

            elapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(elapsed / duration);
            progressBar.fillAmount = fill;

            UpdateMovingImage();

            yield return null;
        }

        progressBar.fillAmount = 1f;
        UpdateMovingImage();
        OnProgressComplete();
    }

    void UpdateMovingImage()
    {
        if (movingImage == null || progressBar == null) return;

        RectTransform barRect = progressBar.rectTransform;
        RectTransform imageRect = movingImage.rectTransform;

        float xPos = Mathf.Lerp(
            barRect.rect.xMin,
            barRect.rect.xMax,
            progressBar.fillAmount);

        imageRect.DOLocalMoveX(xPos, 0.1f).SetEase(Ease.Linear);
    }

    void OnProgressComplete()
    {
        SoundManager.Instance.StopBGM();

        isCompleted = true;
        isGameWin = true;

        StartCoroutine(GameWinPopup());

    }
    #endregion
    // ========================================

    public void GameOver()
    {
        SoundManager.Instance.StopBGM();

        if (isGameOver) return;
        isGameOver = true;

        StartCoroutine(GameOverPopup());
    }

    IEnumerator GameOverPopup()
    {
        yield return new WaitForSeconds(2f);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            RectTransform panel = gameOverPanel.GetComponent<RectTransform>();
            panel.localScale = Vector3.zero;

            panel.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
            SoundManager.Instance.PlaySFX("LoseMini1");

        }
    }
    IEnumerator GameWinPopup()
    {
        yield return new WaitForSeconds(2f);

        if (gameWinPanel != null)
        {
            gameWinPanel.SetActive(true);

            RectTransform panel = gameWinPanel.GetComponent<RectTransform>();
            panel.localScale = Vector3.zero;

            panel.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
            SoundManager.Instance.PlaySFX("WinMiniGame");

        }
    }
    public void OnContinueWin()
    {
        SoundManager.Instance.PlaySFX("Click");
        FadeOutAndLoadScene("SceneGamePlay");
    }
}
