using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBuddy : MonoBehaviour
{
    [SerializeField] GameObject myShadow;
    [SerializeField] LayerMask layuh;
    GameObject realShadow;
    // Start is called before the first frame update
    void Start()
    {
        realShadow = Instantiate(myShadow);
    }

    // Update is called once per frame
    void Update()
    {
        Ray rayy = new Ray(transform.position, Vector3.down * 100);
        RaycastHit hit;
        Debug.DrawRay(rayy.origin, rayy.direction, Color.green);
        if(Physics.Raycast(rayy, out hit))
        {
            realShadow.transform.position = hit.point;
        }
    }
}
