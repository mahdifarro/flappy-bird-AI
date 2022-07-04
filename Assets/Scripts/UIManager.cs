using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private int highScore = 0;
    public Text scoreText;
    public Text highScoreText;
    public Image gameOverImage;
    private void Start()
    {
        ResetScore();
    }

    public void ResetScore()
    {
        scoreText.text = "";
        highScoreText.text = "";
        gameOverImage.gameObject.SetActive(false);
    }
    public void SetScore(int currentScore)
    {
        if (highScore < currentScore)
        {
            highScore = currentScore;
        }
        scoreText.text = "Score: " + currentScore.ToString();
        highScoreText.text = "High Score: " + highScore.ToString();
        gameOverImage.gameObject.SetActive(true);
    }
}
