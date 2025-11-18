using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Renderer bgRenderer;

    private void Update()
    {
        if (!GameManagerMiniGame1.Instance.isGameStart)
            return;

        if (GameManagerMiniGame1.Instance.isGameOver || GameManagerMiniGame1.Instance.isGameWin)
            return;

        bgRenderer.material.mainTextureOffset += new Vector2(speed * Time.deltaTime, 0);
    }
}
