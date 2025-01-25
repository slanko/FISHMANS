using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.Serialization.Formatters;

/*TODO:
 * CURVE STUFF!!
 * make aiming not framerate dependent
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
    [SerializeField] GameObject p1Bowl, p2Bowl;
    [SerializeField] Transform throwPoint;
    int turnIndex;
    [SerializeField] TextMeshProUGUI currentActionText, currentShotText;
    int currentShot = 1;
    Coroutine powerRoutine, curveRoutine;
    float power, hAngle, vAngle, curve;
    bool canThrow = true, canAim = true, throwing, throwOverride;
    [SerializeField] Slider powerSlider, hAngleSlider, vAngleSlider;
    [SerializeField] GameObject curveSpinner;
    public bool p1;

    //tick rates and change rates - tick rate is how often it increments, and change rate is how much it increments - max frames are how long it lingers on max power.
    [SerializeField] int powerSliderTicRate, powerSliderChangeRate, maxPowerFrames, curveTicRate, curveChange;

    //modifiers
    [SerializeField] float powerMod;

    //follow cam
    [SerializeField] GameObject followCam;
    [SerializeField] FollowCamScript followCamActual;
    [SerializeField] Animator followCamAnim;

    //movement speeds
    [SerializeField] float xRotationSpeed, yRotationSpeed, xChangeAmount, yChangeAmount;

    int p1Bowls, p2Bowls;
    [SerializeField] SpriteSetAnimator spriteSet;

    [SerializeField] List<BowlScript> allBowls;
    [SerializeField] Transform jack;
    [SerializeField] TextMeshProUGUI distText;
    BowlScript activeBowl;
    [SerializeField] GameObject shotUI, activeUI, winUI;

    // Start is called before the first frame update
    void Start()
    {
        currentShotText.text = "SHOT " + currentShot;
    }

    // Update is called once per frame
    void Update()
    {
        if (canThrow || throwOverride)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (powerRoutine == null && curveRoutine == null)
                {
                    Debug.Log("setting power!");
                    powerRoutine = StartCoroutine(powerCoroutine());
                }
                else if (powerRoutine != null && curveRoutine == null)
                {
                    Debug.Log("setting angle!");
                    StopCoroutine(powerRoutine);
                    powerRoutine = null;
                    curveRoutine = StartCoroutine(curveCoroutine());
                }
                else if (curveRoutine != null)
                {
                    Debug.Log("throwin!!");
                    StopCoroutine(curveRoutine);
                    curveRoutine = null;
                    throwBowl();
                }
            }
            if (canAim)
            {
                if (Input.GetKey(KeyCode.W) && vAngleSlider.value > 0) //x is vert and y is horiz 'cause that's how it is on the rotations
                {
                    transform.Rotate(new Vector3(xRotationSpeed * Time.deltaTime, 0, 0));
                    vAngleSlider.value -= xChangeAmount * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.S) && vAngleSlider.value < 100)
                {
                    transform.Rotate(new Vector3(-xRotationSpeed * Time.deltaTime, 0, 0));
                    vAngleSlider.value += xChangeAmount * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.A) && hAngleSlider.value > 0)
                {
                    transform.Rotate(new Vector3(0, -yRotationSpeed * Time.deltaTime, 0));
                    hAngleSlider.value -= yChangeAmount * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.D) && hAngleSlider.value < 100)
                {
                    transform.Rotate(new Vector3(0, yRotationSpeed * Time.deltaTime, 0));
                    hAngleSlider.value += yChangeAmount * Time.deltaTime;
                }
            }
        }
        if (activeBowl != null) distText.text = activeBowl.myDist.ToString("0.00") + "m";
    }

    void throwBowl()
    {
        shotUI.SetActive(false);
        activeUI.SetActive(true);
        canThrow = false;
        currentActionText.text = "BOWLED!!";
        anim.SetTrigger("thrown");
        GameObject toInstantiate;
        if (p1) toInstantiate = p1Bowl;
        else toInstantiate = p2Bowl;
        Rigidbody rb = Instantiate(toInstantiate, throwPoint.position, throwPoint.rotation).GetComponent<Rigidbody>();
        rb.AddForce(rb.transform.forward * powerSlider.value * powerMod, ForceMode.Impulse);
        Vector3 tempCurve = new Vector3(Mathf.Sin(curve * Mathf.Deg2Rad), Mathf.Cos(curve * Mathf.Deg2Rad), 0);
        Debug.Log("Curve = " + curve + " | Angle = " + tempCurve);
        var script = rb.gameObject.GetComponent<BowlScript>();
        activeBowl = script;
        script.curveVector = tempCurve;
        allBowls.Add(script);
        followCam.SetActive(true);
        followCamActual.target = rb.transform;
        followCamAnim.SetTrigger("go");
        //once bowl has come to a rest reset canThrow
    }

    IEnumerator powerCoroutine() //look into making this generic? perhaps... NO!!
    {
        canAim = false;
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

    IEnumerator curveCoroutine()
    {
        currentActionText.text = "CURVE";
        int tic = 0;
        curve = 0;
        bool fafa = false; //fafa will try three times harder!!
        while (!fafa) //THANK YOU, fafa
        {
            tic++;
            if(tic >= curveTicRate)
            {
                tic = 0;
                curve -= curveChange;
                if (curve < 0) curve = 360;
                curveSpinner.transform.rotation = Quaternion.Euler(new Vector3(0, 0, curve));
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void Reset()
    {
        if (p1) p1Bowls++;
        else p2Bowls++;
        if (p2Bowls < 4)
        {
            followCamActual.target = null;
            followCam.SetActive(false);
            canAim = true;
            canThrow = true;
            p1 = !p1;
            spriteSet.p1 = p1;
            transform.rotation = Quaternion.identity;
            hAngleSlider.value = 50f;
            vAngleSlider.value = 50f;
            powerSlider.value = 50f;
            curveSpinner.transform.rotation = Quaternion.identity;
            shotUI.SetActive(true);
            activeUI.SetActive(false);
            currentShot++;
            currentShotText.text = "SHOT " + currentShot;
            currentActionText.text = "READY?";
        }
        else checkWinners();


    }

    void checkWinners()
    {
        List<BowlScript> tempList = new List<BowlScript>();
        //we need to order the list based on closest...
        //UH OH. SORTING ALGORITHM.
        for(int i = 0; i < allBowls.Count - 1; i++)
        {
            //take index. check against the list.
            //if nothing in list, just add it.
            if (tempList.Count == 0) tempList.Add(allBowls[i]);
            else
            {
                bool added = false;
                for(int i2 = 0; i2 <= tempList.Count - 1; i2++)
                {
                    if (tempList[i2].myDist > allBowls[i].myDist)
                    {
                        tempList.Insert(i2, allBowls[i]);
                        added = true;
                        break;
                    }
                }
                if (!added) tempList.Add(allBowls[i]);
            }
        }
        allBowls = tempList;
        //SORTED!!
        //now - divining POINTS from this.
        int winnar = 0, score = 0;
        for (int i3 = 0; i3 < allBowls.Count - 1; i3++)
        {
            if (i3 == 0) //i3 more like :3
            { //man shut up
                winnar = allBowls[0].playerNum;
                score++;
            }
            else if (allBowls[i3].playerNum == winnar) score++;
            else break;
        }
        followCamActual.target = jack;
        followCam.SetActive(true);
        followCamAnim.SetTrigger("go");
        if(winnar == 1) currentActionText.text = "<color=#B3E6FF>PLAYER ONE</color>";
        if(winnar == 2) currentActionText.text = "<color=#FFC2EE>P2 WINNAR</color>";
        currentShotText.text = score + " POINTZ";
        shotUI.SetActive(false);
        activeUI.SetActive(false);
        winUI.SetActive(true);

    }
}
