using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitThereAndChillOutScript : MonoBehaviour
{
    [SerializeField] string[] chillStuffToSay;
    void Start()
    {
        Debug.Log(chillStuffToSay[Random.Range(0, chillStuffToSay.Length)]);
    }
}
