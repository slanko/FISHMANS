using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillboarder : MonoBehaviour
{
    GameObject cam;
    [SerializeField] int type; //0 is normal 1 is preview 2 is follow
    // Start is called before the first frame update
    void Start()
    {
        if (type == 0) cam = GameObject.Find("Main Camera");
        else if (type == 1) cam = GameObject.Find("Preview Cam");
        else cam = GameObject.Find("Follow Cam");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam.transform.position);
    }
}
