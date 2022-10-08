using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveCollision : MonoBehaviour
{
    public Terrain Terrain;
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.transform.tag == "Fox")
        {
            Debug.Log("Terrain off");
            Terrain.gameObject.GetComponent<TerrainCollider>().enabled = false;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.tag == "Fox")
        {
            Debug.Log("Terrain on");
            Terrain.gameObject.GetComponent<TerrainCollider>().enabled = true;
        }
    }
}
