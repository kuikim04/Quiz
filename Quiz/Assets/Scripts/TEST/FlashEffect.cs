using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FlashEffect : MonoBehaviour
{
    public GameObject WishPage;
    public GameObject MorningPage;

    public Image flashImage;
    public Image FadeWhite;
    public float duration = 0.5f;
    public float maxScale = 3f;
  
    public void PlayFlash()
    {
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlaySFX("magic");

        flashImage.gameObject.SetActive(true);
        flashImage.transform.localScale = Vector3.zero;
        flashImage.color = new Color(1, 1, 1, 1);

        flashImage.transform.DOScale(maxScale, duration).SetEase(Ease.OutCubic);
        flashImage.DOFade(0f, duration).SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                flashImage.gameObject.SetActive(false);
                FadeWhite.gameObject.SetActive(true);
                FadeWhite.color = new Color(1, 1, 1, 0);

                SoundManager.Instance.PlaySFX("zoom");
                
                FadeWhite.DOFade(1f, duration).SetEase(Ease.OutCubic)
                    .OnComplete(() =>
                    {
                        SoundManager.Instance.PlayBGM("bird");

                        WishPage.SetActive(false);
                        MorningPage.SetActive(true);


                        FadeWhite.DOFade(0f, duration).SetEase(Ease.OutCubic)
                            .OnComplete(() => FadeWhite.gameObject.SetActive(false));
                    });
            });
    }
}
