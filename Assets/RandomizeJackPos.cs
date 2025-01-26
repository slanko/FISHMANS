using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeJackPos : MonoBehaviour
{
    [SerializeField] float posChangeY, posChangeZ;
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + Random.Range(-posChangeY, posChangeY), transform.position.z + Random.Range(-posChangeZ, posChangeZ));
    }
}
