using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayBottle : MonoBehaviour {

    [Header("Moving")]
    public Camera myCam;
    public float moveSpeed;
    [HideInInspector]
    public bool drawer = false;

    [Header("Force")]
    [Tooltip("Force applied to ants when sprayer is on")]
    public float forceStrength;
    private float forceMultiplier = 1;
    private float baseForce;
    [Tooltip("Increases Force Strength multiplicitivly")]
    public float forceIncreaseRate;
    [Tooltip("Caps the max multiplier")]
    public float forceCap;
    [Tooltip("AoE radius of the Spray Bottle")]
    public float explosionRadius;

    [Header("Water Level")]
    public GameObject waterMove;
    [Tooltip("Maximum water(stamina) from spraybottle")]
    public float waterCap;
    private float currentWater;
    [Tooltip("Rate at which water is drained when LMB is down")]
    public float waterDrainRate;
    [Tooltip("Rate at which water passively regenerates")]
    public float waterRegenRate;
    [Tooltip("Rate at which water Actively regenerates")]
    public float waterRegenActive;
    private bool waterForceStopped;
    [Tooltip("Minimum water needed (in decimal %) to shoot after it was forcefully ended from hitting 0")]
    public float forceStoppedLimit;
    [Tooltip("Turns on RMB click icon when water is low")]
    public float lowWater;
    public GameObject lowWaterIcon;

    private float emptyHeight = -133f;
    private float fullHeight = -11.05f;

    private GameObject sprayEffect;


    // Use this for initialization
    void Start () {
        //Assigns Drawer script and turns off at start
        drawer = gameObject.GetComponent<Drawer>().enabled = false;

        //Force Multiplier
        baseForce = forceMultiplier;

        //SprayEffect
        sprayEffect = transform.GetChild(0).gameObject;
        sprayEffect.SetActive(false);

        //WaterRegen
        currentWater = waterCap;
	}
	
	// Update is called once per frame
	void Update () {
        //MouseMovement
        FollowMouse();

        //Shooting
        Shoot();

        //Keeps nodes from auto corrupting
        gameObject.GetComponent<Drawer>().enabled = drawer;

        //WaterRegen
        WaterRegen();
        //WaterLerpVisual
        WaterVisual();

    }

    private void FollowMouse()
    {
        RaycastHit hit = new RaycastHit();

        Ray vRay = myCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(vRay, out hit))
        {
            transform.position = new Vector3(hit.point.x, 0.1f, hit.point.z);
        }

    }  

    private void Shoot()
    {
        //Water has forcefully been stopped & has enough new water
        if (waterForceStopped && currentWater >= forceStoppedLimit/waterCap)
        {
            waterForceStopped = false;
        }
        if (Input.GetMouseButton(0) && !waterForceStopped && Time.timeScale != 0)
        {
            //Turns on Drawer
            drawer = true;

            //Turns on Particle effects & stream, etc.
            sprayEffect.SetActive(true);

            ///Applies Force to ants
            //Assigns layer mask & gathers all of it's colliders
            int layerMask = 1 << 9;
            Collider[] antColliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);

            //Force multiplier
            forceMultiplier = forceIncreaseRate * forceMultiplier;
            if (forceMultiplier > forceCap)
            {
                forceMultiplier = forceCap;
            }
           
            //For each ant...
            for (int i = 0; i < antColliders.Length; i++)
            {
                Rigidbody rbTarget = antColliders[i].GetComponent<Rigidbody>();
                if (!rbTarget)
                {
                    continue;
                }
                //Applies force
                rbTarget.AddExplosionForce(forceStrength * forceMultiplier, transform.position, explosionRadius);

                //Stops ants painting
                rbTarget.gameObject.GetComponent<AntBehavior>().isSplashed = true;
                rbTarget.gameObject.GetComponent<AntBehavior>().CanDrawChecker(gameObject, explosionRadius);
            }

            //Consume Water
            DrainWater();
        }
        if (Input.GetMouseButtonUp(0))
        {
            StopWater();
        }
    }

    private void StopWater()
    {
        //Turns off Drawer
        drawer = false;

        //Resets multiplier
        forceMultiplier = baseForce;

        //Turns off Particle effects
        sprayEffect.SetActive(false);

        //Sets ants to paint again
        foreach (GameObject ant in GameObject.FindGameObjectsWithTag("Ant"))
        {
            ant.GetComponent<AntBehavior>().isSplashed = false;
            ant.GetComponent<AntBehavior>().CanDraw();

        }
    }

    private void DrainWater()
    {
        //Checks if no water is left
        if (currentWater <= 0)
        {
            //Bugfix
            currentWater = 0;
            //Stops water
            StopWater();
            waterForceStopped = true;

            //Water Full Pos height set
            waterMove.transform.localPosition = new Vector3(0f, emptyHeight, 0f);

        }
        //Drains Water
        else
        {
            //Reduces current water level
            currentWater -= waterDrainRate*Time.deltaTime;
        }
    }

    private void WaterRegen()
    {
        //Passive Water Regen
        if (!Input.GetMouseButton(0))
        {
            //If water is not full
            if (currentWater < waterCap)
            {
                //Regen
                currentWater += waterRegenRate * Time.deltaTime;

            }
            //Water is full
            else
            {
                currentWater = waterCap;

                //Water Full Pos height set
                waterMove.transform.localPosition = new Vector3(0f, fullHeight, 0f);
            }
        }

        //Active Water Regen
        if (Input.GetMouseButtonDown(1))
        {
            currentWater += waterRegenActive;
        }
    }

    private void WaterVisual()
    {
        //Water height change Maths
        float multiY = (fullHeight - emptyHeight) / waterCap;
        float currentY = multiY * currentWater + emptyHeight;
        float yPos = Mathf.Lerp(waterMove.transform.localPosition.y, currentY, 6 * Time.deltaTime);

        //Change Height
        waterMove.transform.localPosition = new Vector3(0f, yPos, 0f);

        ///RMB visual
        ///When LMB Down      
        if (Input.GetMouseButton(0))
        {
            RMBVFX(lowWater);
        }
        ///When LMB Up
        else
        {
            RMBVFX(lowWater * 3.25f);
        }
    }

    private void RMBVFX(float threshold)
    {
        //If water is above a certain amount
        if (currentWater / waterCap > threshold || Time.timeScale != 1)
        {
            //Turns off Image
            lowWaterIcon.SetActive(false);

        }
        //If water is below a certain amount
        else
        {
            //Turns on Image
            lowWaterIcon.SetActive(true);
        }
    }
}
