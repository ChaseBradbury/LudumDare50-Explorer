using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreenUI : MonoBehaviour
{

    [SerializeField]
    private Text scoreText;

    [SerializeField]
    private Text scoreMessage;

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }

    public void MakeHighscore()
    {
        scoreMessage.text = "New Highscore!";
    }
}
