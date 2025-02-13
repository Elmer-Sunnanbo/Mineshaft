using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecordingUtils : MonoBehaviour
{
    [SerializeField] List<GameObject> UIItems = new List<GameObject>();
    bool UIActive;
    void Start()
    {
        
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            UIActive = !UIActive;
            ToggleUI(UIActive);
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
            if (item.TryGetComponent(out TextMeshPro tmp))
            {
                tmp.enabled = toggle;
            }
        }
    }
}
