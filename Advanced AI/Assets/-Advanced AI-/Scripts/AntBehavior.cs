using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntBehavior : MonoBehaviour {

    //Recieved from Spawner
    //[HideInInspector]
    public GameObject path;
    //[HideInInspector]
    public GameObject targetNode;
    //[HideInInspector]
    public int currentNode;
    private GameObject oldPath;
    private List<GameObject> pathNodes = new List<GameObject>();
    public bool moveLeft;

    [Header("Splashed")]
    public bool isSplashed = false;
    public float sprayDistance = 1.25f;

    [Header("Movement")]
    private Rigidbody rb;
    public float acceleration;
    public float maxSpeed;

    [HideInInspector]
    public GameObject spray;
    private bool waterRun;
    private bool runFromEdge;

    [Header("Exclamation Point")]
    public GameObject exclamation;
    public GameObject pinch;
    public Quaternion exRot;

    void Awake()
    {
        //Movement
        rb = GetComponent<Rigidbody>();

        //Picks randomly whether ant starts left or right
        #region Set Move Left/Right

        int startMove = Random.Range(0, 2);
        if (startMove != 0)
        {
            moveLeft = true;
        }
        else
        {
            moveLeft = false;
        }

        #endregion
    }

    void Start ()
    {
        //Adds all nodes to a readable list
        for (int i = 0; i < path.transform.childCount; i++)
        {
            pathNodes.Add(path.transform.GetChild(i).gameObject);
        }

        //Sets the target node to the spawned node
        newTargetNode();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Movement();      
    }
    void Update()
    {
        //Transform Position of !
        if (currentEx != null)
        {
            currentEx.transform.position = transform.position;
        }
        //Turns off ! when not being splashed
        if (!isSplashed)
        {
            Destroy(currentEx);
        }
        //Destroys when falls far enough
        if (transform.position.y <= -100f)
        {
            Destroy(gameObject);
        }

        ///Pinch Mark
        //If is being splashed but not running
        if (isSplashed && currentEx == null)
        {
            pinch.SetActive(true);
        }
        else
        {
            pinch.SetActive(false);
        }

        //Getting hit by water
        HitByWater();

        if (transform.GetChild(0).GetComponent<Drawer>().paintAble == false &&
            GameObject.FindGameObjectWithTag("Spray").transform.GetChild(0).gameObject.activeSelf == false)
        {
            CanDraw();
        }
    }

    private void Movement()
    {
        //Run away from spray
        if (isSplashed)
        {
            waterRun = true;

            //If not too close to edge
            if (runFromEdge == false)
            {
                //Face away from spray
                transform.LookAt(2 * transform.position - spray.transform.position);
                ///Runs away from spray
                if (rb.velocity.magnitude > maxSpeed)
                {
                    //Caps Velocity
                    rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
                }
                else
                {
                    //Applies Acceleration
                    rb.AddForce(transform.forward * acceleration);
                }
            }


        }
        //Node movement
        else
        {
            //Normal leaf movement
            MovementPath(targetNode.transform);

            //Stop running from water
            waterRun = false;
        }

        //Debug: Keeps ants above leaf
        if (GetComponent<Rigidbody>().useGravity == false)
        {
            transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
        }
        //If falling
        else
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(0, -1000, 0), ForceMode.Acceleration);
        }
    }

    private void MovementPath(Transform lookAt)
    {
        //Faces TargetNode
        transform.LookAt(lookAt);

        //Caps Velocity
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
        else
        {
            //Applies Acceleration
            rb.AddForce(transform.forward * acceleration/2);
        }

        //Moves to next node
        if (Vector3.Distance(transform.position, targetNode.transform.position) <= 0.15f)
        {
            //Corrupts that node
            targetNode.GetComponent<Completed>().corrupted = true;

            //Moves target node
            newTargetNode();
        }

    }

    //Splashed by Hose
    private void HitByWater()
    {
        //Is being hit by hose
        if (isSplashed)
        {
            //Stops ability to draw
            transform.GetChild(0).GetComponent<Drawer>().paintAble = false;

            //Alert Animation

            //Check to keep running
            if (Vector3.Distance(transform.position, spray.transform.position) > sprayDistance)
            {
                isSplashed = false;
            }

        }
    }

    //Allows ants far away to draw again
    public void CanDrawChecker(GameObject spray, float explosionRadius)
    {
        if (Vector3.Distance(transform.position, spray.transform.position) >= explosionRadius*1.45f)
        {
            CanDraw();
        }
    }

    public void CanDraw()
    {
        //Stops ability to draw
        isSplashed = false;
        transform.GetChild(0).GetComponent<Drawer>().paintAble = true;

        //Alert Animation

        //Run Away from hose

    }

    #region ChangingPapths

    private void newTargetNode()
    {
        #region MoveLeft

        if (moveLeft)
        {
            //Increases int
            currentNode = currentNode + 1;
            //Changes path if at the end
            if (currentNode >= pathNodes.Count)
            {
                //int Clamp
                currentNode = currentNode - 1;
                //Change Path
                ChangePath();
            }
            //Changes new target to the next one in the path
            else
            {
                //Adds Ant to old target node
                targetNode.GetComponent<Completed>().ants.Add(this.gameObject);
                //Assigns new target node
                targetNode = pathNodes[currentNode];
            }
        }

        #endregion

        #region MoveRight

        else
        {
            //Increases int
            currentNode = currentNode - 1;
            //Changes path if at the end
            if (currentNode < 0)
            {
                //int Clamp
                currentNode = currentNode + 1;
                //Change Path
                ChangePath();           
            }
            //Changes new target to the next one in the path
            else
            {
                targetNode = pathNodes[currentNode];
                
            }
        }

        #endregion
    }
    private void ChangePath()
    {
        //Completes path
        path.GetComponent<Completed>().corrupted = true;

        //Bugfix sliding
        rb.velocity = Vector3.zero;
        //Changes move direction
        moveLeft = !moveLeft;
        NewPath();
    }

    private List<GameObject> unCorruptedNodes = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> freePaths = new List<GameObject>();

    private void NewPath()
    {
        //Emtpies all free paths
        freePaths.Clear();
        //Adds all free paths to list
        foreach (GameObject path in GameObject.FindGameObjectsWithTag("Path"))
        {
            //For each path without an ant...
            if (path.GetComponent<Completed>().ants.Count == 0)
            {
                //...add to list of free paths
                freePaths.Add(path);
            }
        }
        //Random Spawn if no paths are left
        if (freePaths.Count == 0)
        {
            //Grabs all the nodes that are not overrun
            GameObject[] nodes = GameObject.FindGameObjectsWithTag("TravelNode");
            //Adds nodes to uncorrupted list
            foreach (GameObject node in nodes)
            {
                if (node.GetComponent<Completed>().corrupted == false)
                {
                    unCorruptedNodes.Add(node);
                }
            }

            //Place
            //Picks random path (any)
            int randomPath = Random.Range(0, unCorruptedNodes.Count - 1);
            GameObject pathPicked = freePaths[randomPath];
            AssignPath(pathPicked);
        }
        else
        {
            //Place
            //Picks random free path
            int randomPath = Random.Range(0, freePaths.Count - 1);
            GameObject pathPicked = freePaths[randomPath];
            AssignPath(pathPicked);
        }

    }

    private void AssignPath(GameObject newPath)
    {
        ///Assigns Node list
        //Clears list
        pathNodes.Clear();
        //Assigns list
        for (int i = 0; i < newPath.transform.childCount; i++)
        {
            pathNodes.Add(newPath.transform.GetChild(i).gameObject);
        }
        //Debug.Log(pathNodes[1].gameObject.transform.parent);

        //Clears Ant from old path
        path.GetComponent<Completed>().ants.Remove(gameObject);
        ///Assigns path
        path = newPath;
        //Clears Ant from old path
        path.GetComponent<Completed>().ants.Add(gameObject);

        ///Assigns targetNode
        //Moving Left
        if (moveLeft)
        {
            targetNode = newPath.transform.GetChild(0).gameObject;
        }
        //Moving Right
        else
        {
            targetNode = newPath.transform.GetChild(path.transform.childCount-1).gameObject;
        }
        
    }

    #endregion

    private void Death()
    {
        //Remove this any from all paths & Nodes
        GameObject[] myNodes = GameObject.FindGameObjectsWithTag("TravelNode");
        GameObject[] myPaths = GameObject.FindGameObjectsWithTag("Path");

        //Removes this any from Node Lists
        foreach (GameObject node in myNodes)
        {
            List<GameObject> antList = node.GetComponent<Completed>().ants;
            if (antList.Contains(this.gameObject))
            {
                antList.Remove(node);
            }
        }
        //Removes this any from Path Lists
        foreach (GameObject paths in myPaths)
        {
            List<GameObject> antList = paths.GetComponent<Completed>().ants;
            if (antList.Contains(this.gameObject))
            {
                antList.Remove(paths);
            }
        }

        //Turns off !
        Destroy(currentEx);

        //Deletes this object
        GetComponent<Rigidbody>().useGravity = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        //Node Movement
        if (other.tag == "TravelNode" && pathNodes.Contains(other.gameObject))
        {
            other.GetComponent<Completed>().ants.Add(this.gameObject);
        } 
    }

    private void OnTriggerStay(Collider other)
    {
        //Edge of leaf
        if (other.tag == "Leaf Edge" && waterRun)
        {
            //Toggles to run from edge
            runFromEdge = true;

            ////Turns on alert
            ExclamationPoint(true);

            #region Run

            //Face the spray
            transform.LookAt(spray.transform.position);
            ///Runs to spray
            if (rb.velocity.magnitude > maxSpeed)
            {
                //Caps Velocity
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
            }
            else
            {
                //Applies Acceleration
                rb.AddForce(transform.forward * acceleration);
            }

            #endregion
        }
    }

    private GameObject currentEx;

    private void ExclamationPoint(bool on)
    {
        //Turns on !
        if (on)
        {
            //If there is no !
            if (currentEx == null)
            {
                currentEx = Instantiate(exclamation);
            }
        }
        //Turns off !
        else
        {
            Destroy(currentEx);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Fall off leaf
        if (other.tag == "Terrain")
        {
            Death();
        }
        //Edge of leaf
        else if (other.tag == "Leaf Edge" && isSplashed == false)
        {
            waterRun = false;
            runFromEdge = false;

            //Turns off alert
            ExclamationPoint(false);
        }
    }
}
