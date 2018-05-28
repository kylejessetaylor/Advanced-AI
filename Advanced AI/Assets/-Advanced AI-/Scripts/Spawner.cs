﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject travelNodes;

    //Ant Spawning
    [Header("Ants")]
    public GameObject ant;
    public float spawnRate;
    private float timer;
    public int startNumber;

	// Use this for initialization
	void Start () {
        SpawnAnt();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void SpawnTiming()
    {
        if (Time.timeSinceLevelLoad - timer >= spawnRate)
        {
            //Spawns Ant
            SpawnAnt();
            //Resets Timer
            timer = Time.timeSinceLevelLoad;
        }
    }

    #region Spawning

    private List<GameObject> unCorruptedNodes = new List<GameObject>();
    private List<GameObject> freePaths = new List<GameObject>();
    private int currentNode;

    private void SpawnAnt()
    {
        //Adds all free paths to list
        foreach(GameObject path in GameObject.FindGameObjectsWithTag("Path"))
        {
            if (path.GetComponent<Completed>().ants.Count == 0)
            {
                freePaths.Add(path);
            }
        }

        //Random Spawn if no paths are left
        if (freePaths.Count == 0)
        {
            //Grabs all the nodes that are not overrun
            GameObject[] nodes = GameObject.FindGameObjectsWithTag("TravelNode");
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

            //Spawn
            Spawn(nodePicked);

            //Preps node element number for Ant's movement behaviour
            currentNode = randomNode;
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

            //Spawn
            Spawn(nodePicked);

            //Preps node element number for Ant's movement behaviour
            currentNode = randomNode;
        }
    }

    private void Spawn(GameObject spawnLocation)
    {
        //Spawns Ant
        GameObject spawnAnt = Instantiate(ant);      
        spawnAnt.transform.position = spawnLocation.transform.position;
        Debug.Log(spawnLocation.transform.position);


        //Assigns Ant to path
        spawnLocation.GetComponentInParent<Completed>().ants.Add(spawnAnt);

        //Assigns targetNode to the Ant
        spawnAnt.GetComponent<AntBehavior>().targetNode = spawnLocation;
        //Assigns path to the Ant
        spawnAnt.GetComponent<AntBehavior>().path = spawnLocation.transform.parent.gameObject;
        //
        spawnAnt.GetComponent<AntBehavior>().currentNode = currentNode;
    }

    #endregion
}