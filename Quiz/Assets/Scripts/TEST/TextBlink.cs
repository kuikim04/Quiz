using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBlink : MonoBehaviour
{
    public TextMeshProUGUI targetText;      
    public float duration = 0.5f; 

    private void Start()
    {
        if (targetText == null) targetText = GetComponent<TextMeshProUGUI>();

        targetText.DOFade(0f, duration)
                  .SetLoops(-1, LoopType.Yoyo)
                  .SetEase(Ease.InOutSine); 
    }
}
