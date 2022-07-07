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
    public Transform Player;
    public Vector3 move;

    public float jumpSpeed;
    public float ySpeed;

    public int[] LeftToCollect;
    public bool[] CollectedAll;
    bool AllCollected;
    public GameObject EndTrigger;

    private void Start()
    {

    }
    void Update()
    {
        //Mouse and player movement
        Hrotate += SpeedH * Input.GetAxis("Mouse X");
        Vrotate -= SpeedV * Input.GetAxis("Mouse Y");
        float HInput = Input.GetAxis("Horizontal");
        float VInput = Input.GetAxis("Vertical");

        //Movement
        move = new Vector3(HInput, 0, VInput) * Speed;


        transform.Translate(move * Time.deltaTime);
        //Rotates the camera seperately to keep the capsule on the y axis and not walk into the floor
        GetComponentInChildren<Camera>().transform.eulerAngles = new Vector3(Vrotate, Hrotate, 0);
        //Rotates the player to where the camera is facing
        transform.eulerAngles = new Vector3(0, Hrotate, 0);

        if(CollectedAll[0] ==  true && CollectedAll[1] == true && CollectedAll[2] == true)
        {
            Debug.Log("You've collected all the food! Head back to the den.");
            AllCollected = true;
            EndTrigger.SetActive(true);
        }
    }


    //This will detect when the box with have the item
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Berry")
        {
            if(CollectedAll[0] == false)
            {
                LeftToCollect[0] -= 1;
                Destroy(collision.gameObject);
                if (LeftToCollect[0] <= 0)
                {
                    CollectedAll[0] = true;
                    Debug.Log("Picked up all the Berries");
                }
                else Debug.Log("Picked up a Berry");
            }
        }
        if (collision.gameObject.tag == "Meat")
        {
            if (CollectedAll[1] == false)
            {
                LeftToCollect[1] -= 1;
                Destroy(collision.gameObject);
                if (LeftToCollect[1] <= 0)
                {
                    CollectedAll[1] = true;
                    Debug.Log("Picked up all the Meat");
                }
                else Debug.Log("Picked up some Meat");
            }
        }
        if (collision.gameObject.tag == "Rubbish")
        {
            if (CollectedAll[2] == false)
            {
                Debug.Log("Picked up some Rubbish");
                LeftToCollect[2] -= 1;
                Destroy(collision.gameObject);
                if (LeftToCollect[2] <= 0)
                {
                    CollectedAll[2] = true;
                    Debug.Log("Picked up all the Rubbish");
                }
                else Debug.Log("Picked up some Rubbish");
            }
        }
        if(collision.gameObject.tag == "Box" && AllCollected == true)
        {
            Debug.Log("You finished the game!");
        }
        if (collision.gameObject.tag == "Bucket")
        {

        }
    }
}
