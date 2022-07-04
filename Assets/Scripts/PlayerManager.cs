using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [SerializeField]
    private GameObject player;
    private Vector3 playerDefaultPose;
    [SerializeField]
    private float velocity = 1;
    
    private bool isGameOn = true;

    private Rigidbody2D player_rb;
    private Animator player_animator;


    private int score = 0;
    private void OnEnable()
    {
        _GameManager.Instance.GameStats_Action += ToggleGame;
    }

    private void OnDisable()
    {
        _GameManager.Instance.GameStats_Action -= ToggleGame;
    }

    // Start is called before the first frame update
    void Awake()
    {
        AssignProperties();
    }


    // Update is called once per frame
    void Update()
    {
        if (isGameOn) {
            DetectInput();
        }        
    }

    void AssignProperties()
    {
        player_rb = player.GetComponent<Rigidbody2D>();
        playerDefaultPose = player.transform.position;
        player_animator = player.GetComponentInChildren<Animator>();
    }

    void ToggleGame(gameStats gameStat)
    {
        if (gameStat == gameStats.start)
        {
            isGameOn = true;
            player.transform.position= playerDefaultPose;
            player_rb.gravityScale = 2;
            LevelManager.Instance.StartGame();
        }
        else
        {
            CheckStats(gameStat);
            isGameOn = false;
        }
    }

    private void CheckStats(gameStats gameStat)
    {
        if (gameStat == gameStats.lose)
        {
            LevelManager.Instance.ToggleLevel(gameStat);
            player_rb.gravityScale = 0;
        }
    }

    void DetectInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }
    void Jump()
    {
        player_rb.velocity = Vector2.up*velocity;
        player_animator.SetTrigger("Jump");
    }

    void GiveScore(bool reset = false)
    {
        if (!reset)
        {
            score += 1;
            Debug.Log(score);
        }
        else
        {
            score = 0;
        }

    }
}
