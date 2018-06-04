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
	}
	
	// Update is called once per frame
	void Update () {
        //MouseMovement
        FollowMouse();

        //Shooting
        Shoot();

        //Keeps nodes from auto corrupting
        gameObject.GetComponent<Drawer>().enabled = drawer;

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
        if (Input.GetMouseButton(0))
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
}
