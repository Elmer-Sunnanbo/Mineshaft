using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScene : MonoBehaviour
{
    public Button winSceneButton;
    void Start()
    {
        winSceneButton.onClick.AddListener(WinSceneReturn);
    }
    public void WinSceneReturn()
    {
        SceneManager.LoadScene(2);
    }
}
