using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Completed : MonoBehaviour {

    public bool corrupted;
    public List<GameObject> ants;

    private void FixedUpdate()
    {
        for(int i = 0; i <= ants.Count - 1; i++)
        {
            if (ants[i] == null)
            {
                ants.Remove(ants[i]);
            }
        }
    }

}
