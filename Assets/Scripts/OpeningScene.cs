using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningScene : MonoBehaviour
{
    private float cutsceneTimer;
    void Start()
    {
        cutsceneTimer = 3;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadMainScene();
        }
        if(cutsceneTimer < 0)
        {
            LoadMainScene();
        }
        cutsceneTimer -= Time.deltaTime;
    }

    public void LoadMainScene()
    {
        Debug.Log("play");
        SceneManager.LoadScene("Main");
    }
}
