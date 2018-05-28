using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntBehavior : MonoBehaviour {

    //Recieved from Spawner
    [HideInInspector]
    public GameObject path;
    [HideInInspector]
    public GameObject targetNode;
    [HideInInspector]
    public int currentNode;

    private List<GameObject> pathNodes = new List<GameObject>();

    public bool moveLeft;
    [Header("Movement")]
    private Rigidbody rb;
    public float moveSpeed;

	// Use this for initialization
	void Start () {
        //Movement
        rb = GetComponent<Rigidbody>();

        //Adds all nodes to a readable list
        for (int i = 0; i < path.transform.childCount; i++)
        {
            pathNodes.Add(path.transform.GetChild(i).gameObject);
        }

        //Picks randomly whether ant starts left or right
        #region Move Left/Right
        int startMove = Random.Range(0, 1);
        if (startMove == 1)
        {
            moveLeft = true;
        }
        else
        {
            moveLeft = false;
        }
        #endregion

        //Sets the target node to the spawned node
        newTargetNode();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //Movement
        MovementPath();


    }

    private void MovementPath()
    {
        transform.LookAt(targetNode.transform);
    }

    private void newTargetNode()
    {
        #region MoveLeft

        if (moveLeft)
        {
            //Increases int
            currentNode = currentNode + 1;
            //Changes path if at the end
            if (currentNode > pathNodes.Count)
            {
                ChangePath();
            }
            //Changes new target to the next one in the path
            else
            {
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
        moveLeft = !moveLeft;
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
