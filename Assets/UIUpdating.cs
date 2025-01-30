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
    public static UIUpdating instance;
    [SerializeField] float FlashDuration;
    float GoldFlashUpRemainingTime;
    float HPFlashUpRemainingTime;
    float HPFlashDownRemainingTime;
    float CoalFlashUpRemainingTime;
    float CoalFlashDownRemainingTime;
    float CoalFlash0RemainingTime;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        playerScript = GameManager.instance.player.GetComponent<Player>();
    }

    void Update()
    {
        hpUI.text = playerScript.playerHP.ToString();
        coalUI.text = GameManager.instance.coal.ToString();
        goldUI.text = GameManager.instance.gold.ToString();

        goldUI.color = Color.white;
        hpUI.color = Color.white;
        coalUI.color = Color.white;
        if (GoldFlashUpRemainingTime > 0)
        {
            GoldFlashUpRemainingTime -= Time.deltaTime;
            goldUI.color = new Color(1 - GoldFlashUpRemainingTime / FlashDuration, 1 , 1 - GoldFlashUpRemainingTime / FlashDuration);
        }
        if (HPFlashUpRemainingTime > 0)
        {
            HPFlashUpRemainingTime -= Time.deltaTime;
            hpUI.color = new Color(1 - HPFlashUpRemainingTime / FlashDuration, 1, 1 - HPFlashUpRemainingTime / FlashDuration);
        }
        if (HPFlashDownRemainingTime > 0)
        {
            HPFlashDownRemainingTime -= Time.deltaTime;
            hpUI.color = new Color(1, 1 - HPFlashDownRemainingTime / FlashDuration, 1 - HPFlashDownRemainingTime / FlashDuration);
        }
        if (CoalFlashUpRemainingTime > 0)
        {
            CoalFlashUpRemainingTime -= Time.deltaTime;
            coalUI.color = new Color(1 - CoalFlashUpRemainingTime / FlashDuration, 1, 1 - CoalFlashUpRemainingTime / FlashDuration);
        }
        if (CoalFlashDownRemainingTime > 0)
        {
            CoalFlashDownRemainingTime -= Time.deltaTime;
            coalUI.color = new Color(1, 1 - CoalFlashDownRemainingTime / FlashDuration, 1 - CoalFlashDownRemainingTime / FlashDuration);
        }
        if (CoalFlash0RemainingTime > 0)
        {
            CoalFlash0RemainingTime -= Time.deltaTime;
            coalUI.color = new Color(1 - (CoalFlash0RemainingTime / (FlashDuration*3))/1.5f, 1 - CoalFlash0RemainingTime / (FlashDuration * 3), 1 - CoalFlash0RemainingTime / (FlashDuration * 3));
        }
    }

    public void FlashGoldUp()
    {
        GoldFlashUpRemainingTime = FlashDuration;
    }

    public void FlashHPUp()
    {
        HPFlashUpRemainingTime = FlashDuration;
    }

    public void FlashHPDown()
    {
        HPFlashDownRemainingTime = FlashDuration;
    }

    public void FlashCoalUp()
    {
        CoalFlashUpRemainingTime = FlashDuration;
    }

    public void FlashCoalDown()
    {
        CoalFlashDownRemainingTime = FlashDuration;
    }

    public void FlashCoal0()
    {
        CoalFlash0RemainingTime = FlashDuration * 3;
    }
}
