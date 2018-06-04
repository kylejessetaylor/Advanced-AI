using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    private GameObject[] paths;

	// Use this for initialization
	void Start () {
        //Cursor.visible = false;
        paths = GameObject.FindGameObjectsWithTag("Path");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void CorruptedNodes()
    {
        for (int i = 0; i < paths.Length; i++)
        {
            GameObject path = paths[i];
            if (path.GetComponent<Completed>().corrupted == false)
            {
                break;
            }
            else
            {
                LoseCondition();
            }
        }
    }

    private void LoseCondition()
    {

    }
}
