using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerMiniGame2 : MonoBehaviour
{
    public static GameManagerMiniGame2 Instance;

    public bool isGameStart = false;
    public bool isGameWin = false;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
