using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public Button GameButton;

    // Update is called once per frame
    void Start()
    {
        // Listener for the button to call LoadMainScene when clicked
        GameButton.onClick.AddListener(LoadMainScene);
    }
    
    public void LoadMainScene()
    {
        Debug.Log("play");
        SceneManager.LoadScene("OpeningScene");
    }
}
