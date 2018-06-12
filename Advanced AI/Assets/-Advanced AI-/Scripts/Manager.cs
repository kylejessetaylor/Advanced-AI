using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    private GameObject[] paths;

    [Header("Win Condition")]
    public float gameDuration = 60f;

    private void Awake()
    {
        //Freezes timescale
    }

    void Start () {
        //Cursor.visible = false;
        paths = GameObject.FindGameObjectsWithTag("Path");
    }
	
	// Update is called once per frame
	void Update () {
        //Check for Loss
        CorruptedNodes();

        //Win Game after X seconds
        WinCondition();
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
        Debug.Log("You Lose");

        //Freezes Game
        Time.timeScale = 0;

        //Turns on leaf texture splat object
        //Changes color of painter to Red(Corrupt Leaf)

        //Turns on "Your leaf has been taken from you!" text

    }

    private void WinCondition()
    {
        if (Time.timeSinceLevelLoad >= gameDuration)
        {
            Debug.Log("You Win");

            //Freezes Game
            Time.timeScale = 0;

            //Turns on leaf texture splat object
            //Changes color of painter to Black(Green Leaf)

            //Turns on "Your leaf stays with you!" text

        }
    }
}
