using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillboarder : MonoBehaviour
{
    GameObject cam;
    [SerializeField] bool previewObject;
    // Start is called before the first frame update
    void Start()
    {
        if (!previewObject) cam = GameObject.Find("Main Camera");
        else cam = GameObject.Find("Preview Cam");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam.transform.position);
    }
}
