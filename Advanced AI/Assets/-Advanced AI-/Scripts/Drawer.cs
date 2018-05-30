using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShaderForge;

public class Drawer : MonoBehaviour {

    public GameObject gameManager;

    #region DrawingCollision

    //Paintable
    public Color paintColor;
    public bool sprayBottle;
    private bool paintAble;
    private List<GameObject> collidedNodes = new List<GameObject>();

    //Texture data
    private float objWidth = 7.5f;
    private float objHeight = 15f;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TravelNode" && sprayBottle)
        {
            collidedNodes.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "TravelNode" && sprayBottle)
        {
            collidedNodes.Remove(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //If spray bottle 
        if (sprayBottle && collidedNodes.Count > 0)
        {
            for (int i = 0; i < collidedNodes.Count; i++)
            {
                if (collidedNodes[i].gameObject.GetComponent<Completed>().ants.Count > 0)
                {
                    paintAble = false;
                    break;
                }
                else
                {
                    paintAble = true;
                }
            }
        }
        else
        {
            paintAble = true;
        }

        //Clears corruption bool from travelNode
        if (other.tag == "TravelNode" && paintAble == true)
        {
            //Cleanses Node
            other.GetComponent<Completed>().corrupted = false;
        }
        //Bug's painting
        if (sprayBottle == false)
        {
            paintAble = true;
        }

        if (other.tag == "Terrain" && paintAble == true)
        {
            //Grabs texture from Game Manager
            GameObject terrain = gameManager.GetComponent<Spawner>().leaf;
            Texture2D texture = gameManager.GetComponent<Spawner>().paintTexture;
                    

            //Gets location of the pixel on terrain at this object's pivot &&& Paints a color around the location
            PaintAround(texture, PixelLocation(terrain, texture, objWidth, objHeight),
                paintColor, new Vector2(transform.localScale.x, transform.localScale.z));

        }
    }

    private Vector2 PixelLocation(GameObject objectToPaint, Texture2D textureToPaint, float meshWidth, float meshHeight)
    {
        //Calculates difference in position, making the terrain object the center location (0, 0)
        Vector2 pos = new Vector2((transform.position.x - objectToPaint.transform.position.x), (transform.position.z - objectToPaint.transform.position.z));
        //Acts as pivot on mesh is bottom left
        Vector2 movedPiv = new Vector2(pos.x + meshWidth / 2, pos.y + meshHeight / 2);
        //Gets exact pixel location (x/10 * 512)
        Vector2 pixelLocation = new Vector2((movedPiv.x / meshWidth) * textureToPaint.width, (movedPiv.y / meshHeight) * textureToPaint.height);

        return (pixelLocation);
    }

    private void PaintAround(Texture2D tex, Vector2 pixelLocation, Color color, Vector2 mySize)
    {
        float widthRadius = ((mySize.x / 2) / objWidth) * tex.width;
        float heightRadius = ((mySize.y / 2) / objHeight) * tex.height;

        //NOTE: change "x < (int)pixelLocation.x + (int)widthRadius" to follow spherical equation (x^2 + y^2 = objWidth/2 ^ 2) for circle texture splat

        ////Sets each individual pixel around the object to be a color
        for (int x = (int)pixelLocation.x - (int)widthRadius; x < (int)pixelLocation.x + (int)widthRadius; x++)
        {
            for (int y = (int)pixelLocation.y - (int)heightRadius; y < (int)pixelLocation.y + (int)heightRadius; y++)
            {
                tex.SetPixel(-y, x, color);
            }
        }

        tex.Apply();
    }

    #endregion
}
