using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*TODO:
 * CURVE STUFF!!
 * set canvases to world space
 * player switch
 * scoring/checking
 * bubble popping
 * trails/fx
 * sound
 * music
 * AIGHT!!
 * 
 * 
 */





public class PlayerScript : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] GameObject bowl;
    [SerializeField] Transform throwPoint;
    int turnIndex;
    [SerializeField] TextMeshProUGUI currentActionText;
    Coroutine powerRoutine, hAngleRoutine, vAngleRoutine, curveRoutine;
    float power, hAngle, vAngle, curve;
    bool canThrow = true, throwing, throwOverride;
    [SerializeField] Slider powerSlider, hAngleSlider, vAngleSlider;

    //tick rates and change rates - tick rate is how often it increments, and change rate is how much it increments - max frames are how long it lingers on max power.
    [SerializeField] int powerSliderTicRate, powerSliderChangeRate, maxPowerFrames;

    //modifiers
    [SerializeField] float powerMod;

    //follow cam
    [SerializeField] GameObject followCam;
    [SerializeField] FollowCamScript followCamActual;
    [SerializeField] Animator followCamAnim;

    //movement speeds
    [SerializeField] float xRotationSpeed, yRotationSpeed, xChangeAmount, yChangeAmount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canThrow || throwOverride)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (powerRoutine == null)
                {
                    powerRoutine = StartCoroutine(powerCoroutine());
                }
                else if (powerRoutine != null)
                {
                    StopCoroutine(powerRoutine);
                    powerRoutine = null;
                    throwBowl();
                }
            }
            if (Input.GetKey(KeyCode.W) && vAngleSlider.value > 0) //x is vert and y is horiz 'cause that's how it is on the rotations
            {
                transform.Rotate(new Vector3(xRotationSpeed * Time.deltaTime, 0, 0));
                vAngleSlider.value -= xChangeAmount;
            }
            if (Input.GetKey(KeyCode.S) && vAngleSlider.value < 100)
            {
                transform.Rotate(new Vector3(-xRotationSpeed * Time.deltaTime, 0, 0));
                vAngleSlider.value += xChangeAmount;
            }
            if (Input.GetKey(KeyCode.A) && hAngleSlider.value > 0)
            {
                transform.Rotate(new Vector3(0, -yRotationSpeed * Time.deltaTime, 0));
                hAngleSlider.value -= yChangeAmount;
            }
            if (Input.GetKey(KeyCode.D) && hAngleSlider.value < 100)
            {
                transform.Rotate(new Vector3(0, yRotationSpeed * Time.deltaTime, 0));
                hAngleSlider.value += yChangeAmount;
            }
        }



    }

    void throwBowl()
    {
        canThrow = false;
        currentActionText.text = "BOWLED!!";
        anim.SetTrigger("thrown");
        Rigidbody rb = Instantiate(bowl, throwPoint.position, throwPoint.rotation).GetComponent<Rigidbody>();
        rb.AddForce(rb.transform.forward * powerSlider.value * powerMod, ForceMode.Impulse);
        followCam.SetActive(true);
        followCamActual.target = rb.transform;
        followCamAnim.SetTrigger("go");
        //once bowl has come to a rest reset canThrow
    }

    IEnumerator powerCoroutine() //look into making this generic? perhaps...
    {
        currentActionText.text = "POWER";
        powerSlider.value = 0;
        bool reverse = false;
        int tic = 0;
        int maxTic = 0;
        anim.SetTrigger("throw");
        while(powerSlider.value < 100)
        {
            tic++;
            if(tic == powerSliderTicRate)
            {
                powerSlider.value += powerSliderChangeRate;
                tic = 0;
            }
            yield return new WaitForFixedUpdate();
        }

        while(powerSlider.value >= 100 && maxTic < maxPowerFrames)
        {
            powerSlider.value = 100;
            maxTic++;
            yield return new WaitForFixedUpdate();
        }
        if (maxTic == maxPowerFrames) reverse = true;

        while(powerSlider.value > 0 && reverse)
        {
            tic++;
            if (tic == powerSliderTicRate)
            {
                powerSlider.value -= powerSliderChangeRate;
                tic = 0;
            }
            yield return new WaitForFixedUpdate();
        }
        powerRoutine = StartCoroutine(powerCoroutine());

    }
}
