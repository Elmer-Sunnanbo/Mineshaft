using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackColliScript : MonoBehaviour
{
    [SerializeField] MinecartInteraction mainTrigger;
    // Start is called before the first frame update
    bool isInTriggerBack;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isInTriggerBack && Input.GetKeyDown(KeyCode.E) && mainTrigger.isInTrigger == false)
        {
            //Do shit
            Debug.Log("you are in my Back");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player playerScript))
        {
            isInTriggerBack = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player playerScript))
        {
            isInTriggerBack = false;
        }
    }
}