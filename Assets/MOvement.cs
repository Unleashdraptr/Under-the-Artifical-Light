using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOvement : MonoBehaviour
{
    public int Speed;
    public Transform Player;
    public Vector3 move;
    void Start()
    {
        
    }

    void Update()
    {
        move = new Vector3(Input.GetAxis("Horizontal"), 1, Input.GetAxis("Vertical")) * Time.deltaTime * Speed;
        Player.transform.Translate(move);
    }
}
