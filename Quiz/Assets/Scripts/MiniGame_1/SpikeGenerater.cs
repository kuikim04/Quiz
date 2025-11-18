using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeGenerater : MonoBehaviour
{
    [Header("Spike Settings")]
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private int poolSize = 10;

    [Header("Speed Settings")]
    [SerializeField] private float minSpeed = 3f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float speedIncreaseRate = 0.2f;

    [HideInInspector] public float currentSpeed;

    private readonly List<GameObject> spikePool = new();

    private void Start()
    {
        currentSpeed = minSpeed;
        InitializePool();

        // สร้าง spike อันแรก ณ จุดเริ่มต้น
        GenerateSpike(transform.position);
    }

    // ---------------------------
    // Object Pool Initialization
    // ---------------------------
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject spike = Instantiate(spikePrefab);
            spike.SetActive(false);

            SpikeScripts spikeScript = spike.GetComponent<SpikeScripts>();
            spikeScript.spikeGenerater = this;

            spikePool.Add(spike);
        }
    }

    // ---------------------------
    // Spawn Spike
    // ---------------------------
    public void GenerateSpike(Vector3 spawnPosition)
    {
        if (GameManagerMiniGame1.Instance.isGameOver ||
            GameManagerMiniGame1.Instance.isGameWin)
            return;

        GameObject spike = GetPooledSpike();

        spike.transform.position = spawnPosition;
        spike.transform.rotation = Quaternion.Euler(0, 180, 0);
        spike.SetActive(true);

        IncreaseSpeed();
    }

    // ---------------------------
    // Get Object From Pool
    // ---------------------------
    private GameObject GetPooledSpike()
    {
        foreach (var s in spikePool)
        {
            if (!s.activeInHierarchy)
                return s;
        }

        // pool ไม่พอ → สร้างใหม่ (รองรับเกมเร็วขึ้น)
        GameObject newSpike = Instantiate(spikePrefab);
        newSpike.SetActive(false);

        SpikeScripts spikeScript = newSpike.GetComponent<SpikeScripts>();
        spikeScript.spikeGenerater = this;

        spikePool.Add(newSpike);
        return newSpike;
    }

    // ---------------------------
    // Speed Up
    // ---------------------------
    private void IncreaseSpeed()
    {
        if (currentSpeed < maxSpeed)
        {
            currentSpeed = Mathf.Min(currentSpeed + speedIncreaseRate, maxSpeed);
        }
    }

}
