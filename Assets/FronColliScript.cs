using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FronColliScript : MonoBehaviour
{
    [SerializeField] MinecartInteraction mainTrigger;
    // Start is called before the first frame update
    bool isInTriggerFront;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isInTriggerFront && Input.GetKeyDown(KeyCode.E) && mainTrigger.isInTrigger == false)
        {
            //Do shit
            Debug.Log("you are in my front");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player playerScript))
        {
            isInTriggerFront = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player playerScript))
        {
            isInTriggerFront = false;
        }
    }
}
