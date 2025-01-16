using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int gold;
    public int coal;
    public int gameScore;
    public int GameHighscore;
    Player getPlayer;
    private int playerHealth;
    public GameObject player;

    public TextMeshProUGUI ScoreDisplay;
    private void Awake()
    {
        instance = this;
    }
    void SaveHighscore()
    {
        PlayerPrefs.SetInt("gameHighscore", GameHighscore);
    }
    void GetHighscore()
    {
        GameHighscore = PlayerPrefs.GetInt("gameHighscore");
    }
    private void Start()
    {
        ScoreDisplay = GetComponent<TextMeshProUGUI>();
        gold = 0;
        gameScore = 0;
        coal = 0;
        GetHighscore();
    }
    public void Update()
    {
        playerHealth = getPlayer.playerHP;
        gameScore = (coal + (gold * 2) + (playerHealth * gold));
        if(GameHighscore < gameScore)
        {
            GameHighscore = gameScore;
        }
        ScoreDisplay.SetText("[Highscore: " + GameHighscore + "]");
    }
    private void FixedUpdate()
    {
        SaveHighscore();
        
    }
}
