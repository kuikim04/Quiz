using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Transform gridTranform;
    [SerializeField] private Transform spawnCenter;

    private List<Sprite> spriteParis;
    Card firstSelected;
    Card secondSelected;
    int matchCounts;

    [Header("UI")]
    public Image fadeImage;

    [Header("Fade Settings")]
    public float fadeDurationIn = 1f;
    public float fadeDurationOut = 1f;

    public GameObject gameWinPanel;
    private void Start()
    {
        PrepareSprites();

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

    private void PrepareSprites()
    {
        spriteParis = new List<Sprite>();

        List<int> randIndexes = new();
        while (randIndexes.Count < 6)
        {
            int r = Random.Range(0, sprites.Length);
            if (!randIndexes.Contains(r))
                randIndexes.Add(r);
        }

        foreach (int index in randIndexes)
        {
            spriteParis.Add(sprites[index]);
            spriteParis.Add(sprites[index]);
        }

        ShuffleSprites(spriteParis);
    }

    public void CreateCard()
    {
        List<Card> cards = new();

        for (int i = 0; i < spriteParis.Count; i++)
        {
            Card card = Instantiate(cardPrefab, gridTranform);
            card.SetIconSprite(spriteParis[i]);
            card.cardController = this;

            if (card.TryGetComponent<CanvasGroup>(out CanvasGroup cg))
            {
                cg.alpha = 0f;
            }
            else if (card.GetComponent<Image>() is Image img)
            {
                Color c = img.color;
                c.a = 0f;
                img.color = c;
            }

            cards.Add(card);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(gridTranform as RectTransform);

        StartCoroutine(AnimateCardSpawn(cards));
    }

    IEnumerator AnimateCardSpawn(List<Card> cards)
    {
        float delayPerCard = 0.03f;
        float animDuration = 0.35f;

        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];

            Vector3 targetPos = card.transform.localPosition;

            card.transform.localPosition = spawnCenter.localPosition;
            card.transform.localScale = Vector3.one;

            card.transform.DOLocalMove(targetPos, animDuration)
                .SetEase(Ease.OutBack);

            if (card.TryGetComponent<CanvasGroup>(out CanvasGroup cgFade))
            {
                cgFade.DOFade(1f, animDuration); // à¿´ Canvas Group à»ç¹ 1
            }
            else if (card.GetComponent<Image>() is Image imgFade)
            {
                imgFade.DOFade(1f, animDuration); 
            }

            yield return new WaitForSeconds(delayPerCard);
        }
    }

    public void SetSelected(Card card)
    {
        if (!card.isSelected)
        {
            card.Show();

            if(firstSelected == null)
            {
                firstSelected = card;
                return;
            }

            if (secondSelected == null) 
            { 
                secondSelected = card;
                StartCoroutine(CheckMatching(firstSelected, secondSelected));
                firstSelected = null;
                secondSelected = null;
            }
        }
    }

    IEnumerator CheckMatching(Card a, Card b)
    {
        yield return new WaitForSeconds(0.3f);

        if (a.iconSprite == b.iconSprite)
        {
            SoundManager.Instance.PlaySFX("Wink");
            matchCounts++;

            if (matchCounts == spriteParis.Count / 2)
            {
                SoundManager.Instance.StopBGM();

                PrimeTween.Sequence.Create().
                    Chain(PrimeTween.Tween.Scale(gridTranform, Vector3.one * 1.2f, 0.2f, ease: PrimeTween.Ease.OutBack)).
                    Chain(PrimeTween.Tween.Scale(gridTranform, Vector3.one, 0.1f));

                StartCoroutine(GameWinPopup());
            }
        }
        else
        {
            SoundManager.Instance.PlaySFX("Wrong");
            a.Hide();
            b.Hide();
        }
    }


    void ShuffleSprites(List<Sprite> spriteList)
    {
        for (int i = 0; i < spriteList.Count; i++)
        {
            int randomIndex = Random.Range(0, i + 1);

            Sprite temp = spriteList[i];
            spriteList[i] = spriteList[randomIndex];    
            spriteList[randomIndex] = temp;
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

    public void OnContinue()
    {
        SoundManager.Instance.PlaySFX("Click");
        FadeOutAndLoadScene("SceneGamePlay");
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
