using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

    private GameObject[] paths;

    [Header("Win Condition")]
    public float gameDuration = 60f;

    void Awake()
    {
        //Freezes timescale
        Time.timeScale = 0;
        //Turns off EndGame Text
        endGameObject.SetActive(true);
    }

    void Start () {
        //Cursor.visible = false;
        paths = GameObject.FindGameObjectsWithTag("Path");

        //Turns off endgame UI
        endGameUI.SetActive(false);
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
        //for (int i = 0; i < paths.Length; i++)
        //{
        //    GameObject path = paths[i];
        //    if (path.GetComponent<Completed>().corrupted == false)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        LoseCondition();
        //    }
        //}
        GameObject uncorruptPath = null;
        foreach (GameObject path in paths)
        {
            //If path is not corrupted
            if (path.GetComponent<Completed>().corrupted == false)
            {
                uncorruptPath = path;
            }
        }
        //Checks if any uncorrupt path is left
        if (uncorruptPath == null)
        {
            LoseCondition();
        }
    }

    public void StartButton()
    {
        //Resumes timescale
        Time.timeScale = 1;
        //Turns off EndGame Text
        endGameObject.SetActive(false);
        //Turns off Main Menu
        mainMenu.SetActive(false);
        //Debug Leaf
        cleanLeaf.SetActive(false);
    }

    public void RestartButton(string sceneName)
    {
        //Reloads scene
        SceneManager.LoadScene(sceneName);
        //Starts the game straight off
        StartButton();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public GameObject cleanLeaf;

    public GameObject mainMenu;
    public GameObject endGameUI;
    public GameObject endGameObject;

    private void LoseCondition()
    {
        Debug.Log("You Lost at: " + Time.timeSinceLevelLoad);

        //Turns on leaf texture splat object
        endGameObject.SetActive(true);
        //Changes color of painter to Red(Corrupt Leaf)
        GameObject finalLeaf = endGameObject.transform.Find("EndGameDrawer").gameObject;
        finalLeaf.GetComponent<Drawer>().paintColor = Color.red;
        //Turns on "Your leaf has been taken from you!" text
        endGameUI.transform.Find("Title").GetComponent<Text>().text =
            "Your Leaf has been taken from you!";

        //Freezes Game
        Time.timeScale = 0;
    }

    private void WinCondition()
    {
        if (Time.timeSinceLevelLoad >= gameDuration)
        {
            Debug.Log("You Win");

            //Turns on leaf texture splat object
            endGameObject.SetActive(true);
            //Changes color of painter to Black(Green Leaf)
            GameObject finalLeaf = endGameObject.transform.GetChild(0).gameObject;
            finalLeaf.GetComponent<Drawer>().paintColor = Color.black;
            //Turns on "Your leaf stays with you!" text
            endGameUI.SetActive(true);
            //Sets text to "You Win!"
            endGameUI.transform.Find("Title").GetComponent<Text>().text =
                "The leaf is yours!";

            //Freezes Game
            Time.timeScale = 0;

        }
    }
}
