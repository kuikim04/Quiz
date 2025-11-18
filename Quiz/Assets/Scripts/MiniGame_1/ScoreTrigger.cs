using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    private bool hasScored = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasScored && collision.CompareTag("Player"))
        {
            hasScored = true;
            GameManagerMiniGame1.Instance.LoseHeart();
        }
    }

    private void OnEnable()
    {
        hasScored = false;
    }
}
