using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class deathSceneScript : MonoBehaviour
{
    public float timeHighscore;
    GameManager gameManager;

    TextMeshProUGUI ScoreText;
    // Start is called before the first frame update
    void Start()
    {
        ScoreText = GetComponent<TextMeshProUGUI>();
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
        if(gameManager.timeScore < timeHighscore)//If currect time is less than highscore. Change the highscore to the new score.
        {
            timeHighscore = gameManager.timeScore;
            SaveTimeScore();
        }
        ScoreText.SetText("[Current Score: " + gameManager.gameScore.ToString() + "][Highcore: " + gameManager.GameHighscore.ToString() + "][Current Time: " + currentTimescore + "][Fastest Time: " + timeHighscore + "]");
    }
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(0);
        }
    }
}
