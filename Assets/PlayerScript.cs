using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] GameObject bowl;
    [SerializeField] Transform throwPoint;
    int turnIndex;
    [SerializeField] TextMeshProUGUI currentActionText;
    Coroutine powerRoutine, hAngleRoutine, vAngleRoutine, curveRoutine;
    float power, hAngle, vAngle, curve;
    bool throwing;
    [SerializeField] Slider powerSlider, hAngleSlider, vAngleSlider;

    //tick rates and change rates - tick rate is how often it increments, and change rate is how much it increments - max frames are how long it lingers on max power.
    [SerializeField] int powerSliderTicRate, powerSliderChangeRate, maxPowerFrames;

    //modifiers
    [SerializeField] float powerMod;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(powerRoutine == null)
            {
                powerRoutine = StartCoroutine(powerCoroutine());
            }
            else if(powerRoutine != null)
            {
                StopCoroutine(powerRoutine);
                powerRoutine = null;
                throwBowl();
            }
        }
    }

    void throwBowl()
    {
        currentActionText.text = "BOWLED!!";
        anim.SetTrigger("thrown");
        Rigidbody rb = Instantiate(bowl, throwPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0, powerSlider.value * powerMod);
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
