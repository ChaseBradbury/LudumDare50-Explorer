using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreenUI : MonoBehaviour
{
    
    [SerializeField]
    private Button playAgainButton;

    void OnEnable()
    {
        playAgainButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Game");
        });
    }

    void OnDisable()
    {
        playAgainButton.onClick.RemoveAllListeners();
    }
}
