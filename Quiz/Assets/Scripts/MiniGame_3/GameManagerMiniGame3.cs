using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerMiniGame3 : MonoBehaviour
{

    public static GameManagerMiniGame3 Instance;

    [Header("UI")]
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;

    [Header("Progress Bar")]
    public Image progressBar;
    public Image movingImage;
    public float duration = 60f;
    private bool isCompleted = false;

    [Header("Hearts UI")]
    public Transform heartsContainer;
    public GameObject heartPrefab;
    private List<GameObject> hearts = new();

    [Header("Knowledge Panels (10 panels)")]
    public List<GameObject> knowledgePanels = new();

    [Header("Buttons")]
    public Button watchInfoButton;
    public Button knowledgeContinueButton;

    [Header("Cooldown Image")]
    public Image cooldownImage;

    public bool isGameStart = false;
    public bool IsGameOver = false;
    public bool isGameWin = false;

    private GameObject currentKnowledgePanel;

    [Header("UI")]
    public Image fadeImage;

    [Header("Fade Settings")]
    public float fadeDurationIn = 1f;
    public float fadeDurationOut = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SetupHearts();

        gameOverPanel.SetActive(false);

        cooldownImage.fillAmount = 0;
        cooldownImage.gameObject.SetActive(false);
        knowledgeContinueButton.interactable = false;
        knowledgeContinueButton.gameObject.SetActive(false);

        foreach (var p in knowledgePanels)
            p.SetActive(false);

        watchInfoButton.onClick.AddListener(OpenRandomKnowledge);
        knowledgeContinueButton.onClick.AddListener(OnContinue); 
        
        if (fadeImage != null)
        {
            fadeImage.color = Color.black;
            fadeImage.DOFade(0f, fadeDurationIn)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    fadeImage.gameObject.SetActive(false);
                });
        }
    }

    // ================= HEARTS =====================
    void SetupHearts()
    {
        foreach (Transform t in heartsContainer)
            Destroy(t.gameObject);

        hearts.Clear();

        for (int i = 0; i < HeartManager.Instance.maxHearts; i++)
        {
            GameObject h = Instantiate(heartPrefab, heartsContainer);
            hearts.Add(h);
        }

        UpdateHeartsUI();
    }

    public void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].SetActive(i < HeartManager.Instance.currentHearts);
        }
    }

    public void RemoveOneHeart()
    {
        HeartManager.Instance.currentHearts--;

        int index = HeartManager.Instance.currentHearts;
        GameObject h = hearts[index];

        h.transform.DOScale(0, 0.25f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                h.SetActive(false);
                h.transform.localScale = Vector3.one;
            });

        UpdateHeartsUI();

        if (HeartManager.Instance.currentHearts <= 0)
        {
            GameOver();
        }
    }

    // =============== GAME OVER ===============
    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        SoundManager.Instance.StopBGM();

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
    public void StartGame()
    {
        if (isGameStart) return;

        isGameStart = true;

        if (progressBar != null)
            StartCoroutine(FillProgressBar());
    }

    #region ProgressBar
    IEnumerator FillProgressBar()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (!isGameStart) { yield return null; continue; }

            if (isCompleted || IsGameOver) yield break;

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

    // =============== เปิดความรู้แบบสุ่ม ===============
    private void OpenRandomKnowledge()
    {
        watchInfoButton.interactable = false;

        int index = Random.Range(0, knowledgePanels.Count);

        currentKnowledgePanel = knowledgePanels[index];
        currentKnowledgePanel.SetActive(true);

        knowledgeContinueButton.gameObject.SetActive(true);

        StartCoroutine(StartCooldown());
    }

    // =============== คูลดาว 5 วินาที + tween ===============
    IEnumerator StartCooldown()
    {
        cooldownImage.gameObject.SetActive(true);
        knowledgeContinueButton.interactable = false;

        cooldownImage.fillAmount = 1f;

        float cd = 5f;

        cooldownImage.DOFillAmount(0f, cd).SetEase(Ease.Linear);

        yield return new WaitForSecondsRealtime(cd);

        cooldownImage.gameObject.SetActive(false);
        knowledgeContinueButton.interactable = true;
    }

    // =============== เล่นต่อ ===============
    private void OnContinue()
    {
        knowledgeContinueButton.interactable = false;

        HeartManager.Instance.currentHearts++;
        if (HeartManager.Instance.currentHearts > HeartManager.Instance.maxHearts)
            HeartManager.Instance.currentHearts = HeartManager.Instance.maxHearts;

        FadeOutAndLoadScene("SceneGamePlay");
    }
    public void OnContinueWin()
    {
        SoundManager.Instance.PlaySFX("Click");
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
}
