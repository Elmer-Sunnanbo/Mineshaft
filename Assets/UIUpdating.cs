using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIUpdating : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hpUI;
    [SerializeField] TextMeshProUGUI coalUI;
    [SerializeField] TextMeshProUGUI goldUI;
    Player playerScript;
    void Start()
    {
        playerScript = GameManager.instance.player.GetComponent<Player>();
    }

    void Update()
    {
        hpUI.text = playerScript.playerHP.ToString();
        coalUI.text = GameManager.instance.coal.ToString();
        goldUI.text = GameManager.instance.gold.ToString();
    }
}
