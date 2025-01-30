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
        replayButton.onClick.AddListener(ReplayGameScene);
        menuButton.onClick.AddListener(ReturnToMenuScene);
    }
    public void ReplayGameScene()
    {
        SceneManager.LoadScene(0);
    }
    public void ReturnToMenuScene()
    {
        SceneManager.LoadScene(2);
    }
}
