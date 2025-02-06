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
    public Player getPlayer;
    private int playerHealth;
    public GameObject player;
    [SerializeField] GameObject soundPlayer;

    private void Awake()
    {
        getPlayer = player.GetComponent<Player>();
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
    }
    private void FixedUpdate()
    {
        SaveHighscore();
    }

    /// <summary>
    /// Replicates a source playing a sound. Useful for playing a sound when an object will delete itself.
    /// </summary>
    /// <param name="copySource"></param>
    public void PlaySound(AudioSource copySource)
    {
        GameObject currentSoundPlayer = Instantiate(soundPlayer, copySource.transform.position, Quaternion.identity);
        currentSoundPlayer.GetComponent<AudioSource>().clip = copySource.clip;
        currentSoundPlayer.GetComponent<AudioSource>().outputAudioMixerGroup = copySource.outputAudioMixerGroup;
        currentSoundPlayer.GetComponent<AudioSource>().Play();
    }
}
