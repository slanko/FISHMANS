using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlScript : MonoBehaviour
{
    public Vector3 curveVector;
    float topSpeed = 0.001f;
    [SerializeField] float curveMult;
    Rigidbody rb;
    [SerializeField] float check1;
    int waitFrames = 1, waitCurrent;
    [SerializeField] float velMagCutoff;
    public int playerNum;
    [SerializeField] int sleepFrames, sleepFrameCurrent;
    bool slept;
    Transform jack;
    public float myDist;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        jack = GameObject.Find("The Jack").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        myDist = Vector3.Distance(transform.position, jack.position);

        if(waitCurrent < waitFrames)
        {
            waitCurrent++;
        }
        else
        {
            //we doin physics stuff!
            float vel = rb.velocity.magnitude; //is this absolute??
            if (vel > topSpeed) topSpeed = vel;
            float velMag = vel / topSpeed; //this gives us our value out of 1.
                                           //so now we need to apply curve multiplier based on inverse velocity, right?

            //WE GOTTA LERP BETWEEN TWO VECTORS.
            if (1 - velMag < velMagCutoff)
            {
                Vector3 normalizedCurve = curveVector.normalized;
                Vector3 gotCurve = new Vector3(-normalizedCurve.x * curveMult, normalizedCurve.y * curveMult, rb.velocity.z);
                rb.velocity = Vector3.Lerp(transform.TransformDirection(gotCurve), rb.velocity, 1 - velMag);
                check1 = 1 - velMag;

            }
            else
            {
                rb.drag = 3f;
                if(!slept) sleepFrameCurrent++;
            }
            if (sleepFrameCurrent == sleepFrames && !slept)
            {
                slept = true;
                GameObject.Find("char").GetComponent<PlayerScript>().Reset();
            }


        }





    }
}
