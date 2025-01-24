using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamScript : MonoBehaviour
{
    public Transform target;
    [SerializeField] GameObject myCam;
    void Update()
    {
        if (target != null)
        {
            transform.position = target.position;
            myCam.SetActive(true);
        }
        else myCam.SetActive(false);
    }
}
