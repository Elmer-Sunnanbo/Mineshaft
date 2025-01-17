using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneUI : MonoBehaviour
{
    public Button GameButton;
    public TextMeshProUGUI HP;
    public Player player;

    // Update is called once per frame
    void Start()
    {
        // Listener for the button to call LoadMainScene when clicked
        GameButton.onClick.AddListener(LoadTitleScreen);
    }

    private void Update()
    {
        HP.text = ("HP: " + player.playerHP);
    }

    public void LoadTitleScreen()
    {
        Debug.Log("play");
        SceneManager.LoadScene("TitleScreen");
    }
}
