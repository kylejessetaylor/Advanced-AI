using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayBottle : MonoBehaviour {

    [Header("Moving")]
    public Camera myCam;
    public float moveSpeed;
    private bool drawer;

    [Header("Force")]
    [Tooltip("Force applied to ants when sprayer is on")]
    public float forceStrength;
    [Tooltip("AoE radius of the Spray Bottle")]
    public float explosionRadius;
    

    // Use this for initialization
    void Start () {
        //Assigns Drawer script and turns off at start
        drawer = gameObject.GetComponent<Drawer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        //MouseMovement
        FollowMouse();

    }

    private void FollowMouse()
    {
        //Vector3 pos = Input.mousePosition;
        //pos = Camera.main.ScreenToWorldPoint(pos);
        //pos.y = transform.position.y;
        //transform.position = pos;

        RaycastHit hit = new RaycastHit();

        Ray vRay = myCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(vRay, out hit))
        {
            transform.position = new Vector3(hit.point.x, 0.1f, hit.point.z);
        }

    }

    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Turns on Drawer
            drawer = true;

            //Turns on Particle effects & stream, etc.

            ///Applies Force to ants
            Collider[] antColliders = Physics.OverlapSphere(transform.position, explosionRadius, 000000001);

            for (int i = 0; i < antColliders.Length; i++)
            {
                Rigidbody rbTarget = antColliders[i].GetComponent<Rigidbody>();
                if (!rbTarget)
                {
                    continue;
                }
                rbTarget.AddExplosionForce(forceStrength, transform.position, explosionRadius);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            drawer = false;

            //Turns off Particle effects
        }
    }
}
