using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScene : MonoBehaviour
{
    public Button replayButton;
    public Button menuButton;
    void Start()
    {
        replayButton.onClick.AddListener(LoadMainScene);
        menuButton.onClick.AddListener(ReturnToMenu);
    }
    public void LoadMainScene()
    {
        SceneManager.LoadScene(0);
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(2);
    }
}
