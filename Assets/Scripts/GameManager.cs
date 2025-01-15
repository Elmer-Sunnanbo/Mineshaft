using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int gold;
    public int coal;
    public int gameScore;
    public int GameHighscore;
    public float timeScore;
    public GameObject player;
    Player getPlayer;
    private int playerHealth;

    private void Awake()
    {
        instance = this;
    }
    void SaveHighscore()
    {
        PlayerPrefs.SetInt("gameHighscore", GameHighscore);
        PlayerPrefs.SetFloat("timeHighscore", timeScore);
    }
    void GetHighscore()
    {
        GameHighscore = PlayerPrefs.GetInt("gameHighscore");
        timeScore = PlayerPrefs.GetFloat("timeHighscore");
    }
    private void Start()
    {
        gold = 0;
        gameScore = 0;
        timeScore = 0;
        coal = 0;
        GetHighscore();
    }
    public void Update()
    {
        playerHealth = getPlayer.playerHP;
        gameScore = (gold + playerHealth - 4);
        if(GameHighscore < gameScore)
        {
            GameHighscore = gameScore;
        }
    }
    private void FixedUpdate()
    {
        SaveHighscore();
        timeScore += Time.deltaTime;
    }
}
