using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RecordingUtils : MonoBehaviour
{
    [SerializeField] List<GameObject> UIItems = new List<GameObject>();
    [SerializeField] AudioSource music;
    [SerializeField] Light2D bigLight;
    [SerializeField] Camera mainCam;
    bool UIActive = false;
    bool musicActive = false;
    void Start()
    {
        mainCam = Camera.main;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            UIActive = !UIActive;
            ToggleUI(UIActive);
        }
        if(Input.GetKeyDown(KeyCode.M)) 
        {
            musicActive = !musicActive;
            ToggleMusic(musicActive);
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            bigLight.enabled = true;
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            bigLight.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            mainCam.orthographicSize = 40;
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            mainCam.orthographicSize = 5;
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Main");
        }
    }

    void ToggleUI(bool toggle)
    {
        foreach (GameObject item in UIItems)
        {
            if (item.TryGetComponent(out Image image))
            {
                image.enabled = toggle;
            }
            if (item.TryGetComponent(out TextMeshProUGUI tmp))
            {
                tmp.enabled = toggle;
            }
        }
    }

    void ToggleMusic(bool toggle)
    {
        if(toggle)
        {
            music.volume = 0.44f;
        }
        else
        {
            music.volume = 0;
        }
        
    }
}
