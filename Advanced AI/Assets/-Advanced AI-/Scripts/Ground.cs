using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Renderer rend = GetComponent<Renderer>();

        // duplicate the original texture and assign to the material
        Texture2D texture = Instantiate(rend.material.mainTexture) as Texture2D;
        rend.material.mainTexture = texture;
    }

}
