using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class _GameManager : MonoBehaviour
{
    public UIManager uIManager;

    public static _GameManager Instance;
    public Action<gameStats> GameStats_Action;

    [SerializeField]
    private string obstacleTag;

    private gameStats _currentStats;
    private bool isGamePaused;


    private float score = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        RunGame();
    }

    private void Update()
    {
        GiveScore();
    }

    void GiveScore()
    {
        if(_currentStats == gameStats.running)
        {
            score += Time.deltaTime;
        }
        else
        {
            score = 0;
        }

    }

    void RunGame()
    {
        uIManager.ResetScore();

        isGamePaused = false;
        _currentStats = gameStats.start;
        GameStats_Action?.Invoke(_currentStats);
        _currentStats = gameStats.running;
    }

    void LoseGame()
    {
        uIManager.SetScore((int)score);

        _currentStats = gameStats.lose;
        InvokeGameStats();
        StartCoroutine(ResetGame());
    }

    void WinGame()
    {
        _currentStats= gameStats.win;
        InvokeGameStats();
    }

    void TogglePause()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            _currentStats = gameStats.pause;
        }
        else
        {
            _currentStats = gameStats.running;
        }
        InvokeGameStats();
    }

    IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(1f);
        RunGame();
    }

    void InvokeGameStats()
    {
        GameStats_Action?.Invoke(_currentStats);
    }
    public void HitObject(string objectTag)
    {
        if (objectTag == obstacleTag)
        {
            LoseGame();
        }
    }
}
