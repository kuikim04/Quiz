using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameUI : MonoBehaviour
{
    public static MainGameUI instance;
    [Header("UI")]
    public Image fadeImage;
    public Image fadeImageWhite;
    public Image background;
    public GameObject panelQuiz;
    public GameObject[] firework;
    public GameObject Heart;
    public GameObject reminingtext;
    public CanvasGroup panelWin;

    [Header("Fade Settings")]
    public float fadeDurationIn = 1f;
    public float fadeDurationOut = 1f;

    public bool IsGameWin = false;

    public GameObject gameOverPanel;
    [Header("Knowledge Panels (10 panels)")]
    public List<GameObject> knowledgePanels = new();

    [Header("Buttons")]
    public Button watchInfoButton;
    public Button knowledgeContinueButton;

    [Header("Cooldown Image")]
    public Image cooldownImage;

    private GameObject currentKnowledgePanel;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI textOrder1;
    [SerializeField] private TextMeshProUGUI textOrder2;
    [SerializeField] private Button buttonOrder2;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if(!GameFlowManager.Instance.IsOpenFirst)
            SoundManager.Instance.PlayBGM("pixela");

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
                    QuizManager.Instance.timerRunning = true;
                });
        } 
    }

    public void PlayWinAnimation()
    {
        SoundManager.Instance.StopBGM();
        IsGameWin = true;

        buttonOrder2.interactable = false;

        DG.Tweening.Sequence seq = DOTween.Sequence();

        // 1) รอ 1 วิ
        seq.AppendInterval(1f);

        // -------------------------
        // 2) Fade quiz panel ออก + Heart fade ออกพร้อมกัน
        // -------------------------
        CanvasGroup quizGroup = panelQuiz.GetComponent<CanvasGroup>();
        if (quizGroup == null) quizGroup = panelQuiz.AddComponent<CanvasGroup>();

        CanvasGroup heartGroup = Heart.GetComponent<CanvasGroup>();
        if (heartGroup == null) heartGroup = Heart.AddComponent<CanvasGroup>();

        CanvasGroup reminingtextGroup = reminingtext.GetComponent<CanvasGroup>();
        if (reminingtextGroup == null) reminingtextGroup = reminingtext.AddComponent<CanvasGroup>();

        seq.Append(quizGroup.DOFade(0f, 0.8f).SetEase(Ease.InOutSine));
        seq.Join(heartGroup.DOFade(0f, 0.8f).SetEase(Ease.InOutSine));
        seq.Join(reminingtextGroup.DOFade(0f, 0.8f).SetEase(Ease.InOutSine));

        seq.OnComplete(() =>
        {
            panelQuiz.SetActive(false);
            Heart.SetActive(false);
            reminingtext.SetActive(false);
        });

        // 3) ซูม background
        seq.Append(background.rectTransform.DOScale(1.2f, 1f).SetEase(Ease.InOutQuad));
        SoundManager.Instance.PlaySFX("mainZoom");

        // 4) Fade White เข้า
        fadeImageWhite.gameObject.SetActive(true);
        fadeImageWhite.color = new Color(1, 1, 1, 0);
        seq.Append(fadeImageWhite.DOFade(1f, 0.7f).SetEase(Ease.InOutSine));

        // 5) Fade White ออก
        seq.Append(fadeImageWhite.DOFade(0f, 0.7f).SetEase(Ease.InOutSine))
           .OnComplete(() =>
           {
               fadeImageWhite.gameObject.SetActive(false);
           });

        // 6) PanelWin Fade In
        panelWin.gameObject.SetActive(true);
        panelWin.alpha = 0;

        seq.Append(
            panelWin.DOFade(1f, 1f).SetEase(Ease.InOutSine)
        )
        .OnComplete(() =>
        {
            SoundManager.Instance.PlaySFX("MainWin");
            SoundManager.Instance.PlayBGM("firework");

            foreach (GameObject fx in firework)
            {
                if (fx != null)
                    fx.SetActive(true);
            }

            // ======= สร้าง CanvasGroup ให้ปุ่มถ้ายังไม่มี =======
            CanvasGroup buttonGroup = buttonOrder2.GetComponent<CanvasGroup>();
            if (buttonGroup == null) buttonGroup = buttonOrder2.gameObject.AddComponent<CanvasGroup>();

            // reset alpha
            textOrder1.alpha = 0;
            textOrder2.alpha = 0;
            buttonGroup.alpha = 0;

            DG.Tweening.Sequence textSeq = DOTween.Sequence();

            textSeq.Append(textOrder1.DOFade(1f, 1f));

            textSeq.Append(textOrder2.DOFade(1f, 1f));

            textSeq.AppendInterval(5f);

            textSeq.Append(buttonGroup.DOFade(1f, 1f).OnComplete(() =>
            {
                buttonOrder2.interactable = true;
            }));

            textSeq.Play();
        });

    }

    public void GameOver()
    {
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

        GameFlowManager.Instance.IsGameMainOver = false;    
        GameFlowManager.Instance.questionsSinceMinigame = 0;
        FadeOutAndLoadScene("SceneGamePlay");
    }

    public void BackMain()
    {
        GameFlowManager.Instance.IsGameMainOver = false;
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
