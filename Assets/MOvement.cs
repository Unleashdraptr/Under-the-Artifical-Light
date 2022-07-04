using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public int Speed;
    public float SpeedH;
    public float SpeedV;
    float Hrotate;
    float Vrotate;
    public float FallVelocity;
    public bool Grounded;
    public bool Falling;
    public Transform Player;
    public Vector3 move;
    public CharacterController controller;
    void Start()
    {
        controller = GetComponent<CharacterController>();

    }

    void Update()
    {
        Player.transform.Translate(move = new Vector3(Input.GetAxis("Horizontal"), FallVelocity, Input.GetAxis("Vertical")) * Time.deltaTime * Speed);
        Hrotate += SpeedH * Input.GetAxis("Mouse X");
        Vrotate -= SpeedV * Input.GetAxis("Mouse Y");
        //Rotates the camera seperately to keep the capsule on the y axis and not walk into the floor
        GetComponentInChildren<Camera>().transform.eulerAngles = new Vector3(Vrotate, Hrotate, 0);
        //Rotates the player to where the camera is facing
        transform.eulerAngles = new Vector3(0, Hrotate, 0);
    }

    //This will detect when the box with have the item
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Box")
        {
            if(collision.gameObject.GetComponent<Box>().HasItem == true)
            {
                Debug.Log("The item!");
            }
        }
    }
}
