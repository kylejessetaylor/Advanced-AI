using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject leaf;
    [HideInInspector]
    public Texture2D paintTexture;

    public GameObject travelNodes;

    //Ant Spawning
    [Header("Ants")]
    public GameObject ant;
    public int firstSpawnNumber;
    public float spawnRate;
    private float timer;
    public int antCap;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < firstSpawnNumber; i++)
        {
            SpawnAnt();
        }

        //Instantiates 1 copy of Texture2D and has Drawer reference it
        TextureSplat();
	}
	
	// Update is called once per frame
	void Update () {
        SpawnTiming();
	}

    private void SpawnTiming()
    {
        //Gets ant count
        GameObject[] ants = GameObject.FindGameObjectsWithTag("Ant");
        //If too many ants
        if (ants.Length < antCap)
        {
            //Reduces spawnrate based on number of ants
            if (Time.timeSinceLevelLoad - timer >= spawnRate)
            {
                //Spawns Ant
                SpawnAnt();
                //Resets Timer
                timer = Time.timeSinceLevelLoad;
            }
        }
    }

    private void TextureSplat()
    {
        ///Texture 'Getting'
        GameObject terrain = leaf;

        //IDs the texture
        Renderer rend = terrain.GetComponent<Renderer>();
        //Texture2D texture = rend.material.mainTexture as Texture2D;
        paintTexture = Instantiate(rend.material.GetTexture("_DrawMap")) as Texture2D;
        rend.material.SetTexture("_DrawMap", paintTexture);
    }

    #region Spawning

    private List<GameObject> unCorruptedNodes = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> freePaths = new List<GameObject>();
    private int currentNode;

    private void SpawnAnt()
    {
        //Adds all free paths to list
        foreach(GameObject path in GameObject.FindGameObjectsWithTag("Path"))
        {
            if (path.GetComponent<Completed>().ants.Count == 0 &&
                path.GetComponent<Completed>().corrupted == false)
            {
                freePaths.Add(path);
            }
        }

        //Random Spawn if no paths are left
        if (freePaths.Count == 0)
        {
            //Grabs all the nodes that are not overrun
            GameObject[] nodes = GameObject.FindGameObjectsWithTag("TravelNode");
            //Adds nodes to uncorrupted list
            foreach(GameObject node in nodes)
            {
                if (node.GetComponent<Completed>().corrupted == false)
                {
                    unCorruptedNodes.Add(node);
                }
            }

            //Place
            //Picks random path
            int randomPath = Random.Range(0, unCorruptedNodes.Count - 1);
            GameObject pathPicked = freePaths[randomPath];
            //Picks random node
            int randomNode = Random.Range(0, pathPicked.transform.childCount - 1);
            GameObject nodePicked = pathPicked.transform.GetChild(randomNode).gameObject;

            //Preps node element number for Ant's movement behaviour
            currentNode = randomNode;

            //Spawn
            Spawn(nodePicked);
        }
        else
        {
            //Place
            //Picks random path
            int randomPath = Random.Range(0, freePaths.Count - 1);
            GameObject pathPicked = freePaths[randomPath];
            //Picks random node
            int randomNode = Random.Range(0, pathPicked.transform.childCount - 1);
            GameObject nodePicked = pathPicked.transform.GetChild(randomNode).gameObject;

            //Preps node element number for Ant's movement behaviour
            currentNode = randomNode;

            //Corrupts the node they spawn on
            nodePicked.GetComponent<Completed>().corrupted = true;

            //Spawn
            Spawn(nodePicked);
        }
    }

    private void Spawn(GameObject spawnLocation)
    {
        //Spawns Ant
        GameObject spawnAnt = Instantiate(ant);      
        spawnAnt.transform.position = spawnLocation.transform.position;

        //Assigns Ant to path
        spawnLocation.GetComponentInParent<Completed>().ants.Add(spawnAnt);

        //Assigns targetNode to the Ant
        spawnAnt.GetComponent<AntBehavior>().targetNode = spawnLocation;
        spawnLocation.GetComponent<Completed>().ants.Add(spawnAnt);
        //Assigns path to the Ant
        spawnAnt.GetComponent<AntBehavior>().path = spawnLocation.transform.parent.gameObject;
        //Assigns the current node the ant is on
        spawnAnt.GetComponent<AntBehavior>().currentNode = currentNode;

        //Removes path from freepaths list
        freePaths.Remove(spawnLocation.transform.parent.gameObject);
    }

    #endregion
}
