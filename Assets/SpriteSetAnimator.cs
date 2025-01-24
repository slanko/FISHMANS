using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SpriteSetAnimator : MonoBehaviour
{
    public int index;
    [SerializeField] Sprite[] p1Sprites, p2Sprites;
    [SerializeField] SpriteRenderer rend;
    public bool p1;

    // Update is called once per frame
    void Update()
    {
        if (p1) rend.sprite = p1Sprites[index];
        else rend.sprite = p2Sprites[index];
    }
}
