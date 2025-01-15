using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class deathSceneScript : MonoBehaviour
{
    public float timeHighscore;
    float currentTimescore;
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        GetTimeScore();
    }
    void SaveTimeScore()
    {
        PlayerPrefs.SetFloat("TimeHighscore", timeHighscore);
    }
    void GetTimeScore()
    {
        timeHighscore = PlayerPrefs.GetFloat("TimeHighscore");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(0);
        }
        if(gameManager.timeScore < timeHighscore)
        {
            timeHighscore = gameManager.timeScore;
        }
    }
}
