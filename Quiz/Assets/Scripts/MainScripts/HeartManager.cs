using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeartManager : MonoBehaviour
{
    public static HeartManager Instance;

    public int maxHearts = 10;
    public int currentHearts;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (currentHearts == 0)
            currentHearts = maxHearts;
    }

    public bool LoseHeart()
    {
        if (currentHearts <= 0) return false;

        currentHearts--;

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "SceneGamePlay")
        {
            if (currentHearts <= 0)
            {
                GameFlowManager.Instance.GameOver();
                Debug.Log("GAME OVER!");
            }
        }
        
        if (currentHearts <= 0)
        {
            Debug.Log("GAME OVER!");
        }

        return true;
    }

    public void ResetHearts()
    {
        currentHearts = maxHearts;
    }
}
