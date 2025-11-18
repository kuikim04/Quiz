using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScripts : MonoBehaviour
{
    public SpikeGenerater spikeGenerater;
    private bool hasSpawnedNext = false; 

    private void Update()
    {
        if (!GameManagerMiniGame1.Instance.isGameStart)
            return;

        transform.Translate(spikeGenerater.currentSpeed * Time.deltaTime * Vector2.right);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("NextLine") && !hasSpawnedNext)
        {
            hasSpawnedNext = true;

            Vector3 nextPos = transform.position + Vector3.right * spikeGenerater.GetComponent<SpikeGenerater>().GetComponent<Transform>().right.x + Vector3.right * 10f;
            Vector3 spawnPos = new Vector3(transform.position.x + 10f, transform.position.y, transform.position.z);
            spikeGenerater.GenerateSpike(spawnPos);
        }

        else if (collision.CompareTag("FinishLine"))
        {
            hasSpawnedNext = false;
            gameObject.SetActive(false);
        }
    }
}
