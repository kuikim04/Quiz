using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManagerMiniGame3.Instance.IsGameOver || !GameManagerMiniGame3.Instance.isGameStart || GameManagerMiniGame3.Instance.isGameWin)
            return;

        if (collision.CompareTag("Ground"))
        {
            gameObject.SetActive(false);
        }
        else if (collision.CompareTag("Player"))
        {
            SoundManager.Instance.PlaySFX("dmg");
            gameObject.SetActive(false);

            GameManagerMiniGame3.Instance.RemoveOneHeart();
        }
    }

    private void OnEnable()
    {
        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.velocity = Vector2.zero;
        }
    }
}
