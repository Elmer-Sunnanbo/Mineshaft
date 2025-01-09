using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowMan : MonoBehaviour
{
    [SerializeField] GameObject objectToFollow;
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y, -10);
        transform.position = targetPos;
    }
}
