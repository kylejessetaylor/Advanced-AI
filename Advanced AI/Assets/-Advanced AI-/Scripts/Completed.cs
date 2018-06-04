using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Completed : MonoBehaviour {

    public bool corrupted;
    public List<GameObject> ants;

    private void FixedUpdate()
    {
        //Deletes empty List
        for(int i = 0; i <= ants.Count - 1; i++)
        {
            if (ants[i] == null)
            {
                ants.Remove(ants[i]);
            }
        }
        //Removes corrupt list if node isnt corrupted
        if (corrupted)
        {
            //For every node in a path
            for(int i = 0; i < transform.childCount - 1; i++)
            {
                //If a node is not corrupted
                if (transform.GetChild(i).GetComponent<Completed>().corrupted == false)
                {
                    //Set the path to uncorrupted
                    GetComponent<Completed>().corrupted = false;
                    //Stop checking
                    break;
                }
                //If all nodes are corrupted
                else
                {
                    //Set path to corrupt
                    GetComponent<Completed>().corrupted = true;
                }
            }
        }
    }
}
