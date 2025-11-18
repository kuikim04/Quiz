using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFallController : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject fallingObject;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float spawnHeight = 10f;
    [SerializeField] private float spawnRangeX = 3f;

    [Header("Spawn Rate")]
    [SerializeField] private float initialWait = 3f;
    [SerializeField] private float minWait = 0.5f;
    [SerializeField] private float speedUpRate = 0.05f;

    private List<GameObject> objectPool;
    private float currentWait;

    private void Start()
    {
        InitializePool();
        currentWait = initialWait;
        StartCoroutine(SpawnCoroutine());
    }

    private void InitializePool()
    {
        objectPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(fallingObject);
            obj.SetActive(false);
            objectPool.Add(obj);
        }
    }

    private GameObject GetPooledObject()
    {
        foreach (var obj in objectPool)
        {
            if (!obj.activeInHierarchy)
                return obj;
        }

        GameObject newObj = Instantiate(fallingObject);
        newObj.SetActive(false);
        objectPool.Add(newObj);
        return newObj;
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            if (GameManagerMiniGame3.Instance.IsGameOver || GameManagerMiniGame3.Instance.isGameWin)
            {
                ClearPool();
                yield break;
            }

            SpawnObject();
            yield return new WaitForSeconds(currentWait);

            currentWait -= speedUpRate;
            currentWait = Mathf.Max(currentWait, minWait);
        }
    }
    private void ClearPool()
    {
        foreach (var obj in objectPool)
        {
            if (obj != null)
                Destroy(obj);
        }

        objectPool.Clear();
    }


    private void SpawnObject()
    {
        if (!GameManagerMiniGame3.Instance.isGameStart)
            return;

        GameObject obj = GetPooledObject();
        obj.transform.position = new Vector3(Random.Range(-spawnRangeX, spawnRangeX), spawnHeight, 0f);
        obj.SetActive(true);
    }
}
