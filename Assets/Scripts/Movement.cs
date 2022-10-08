using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    CharacterController characterController;
    public Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    bool canMove = true;
    bool isJumping;



    public int[] LeftToCollect;
    public bool[] CollectedAll;
    bool AllCollected;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.visible = false;
    }
    void Update()
    {

        float movementDirectionY = 0f;

        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        float curSpeedX = 0f;
        float curSpeedY = 0f;
        curSpeedX = canMove ? walkingSpeed * Input.GetAxis("Vertical") : 0;
        curSpeedY = canMove ? walkingSpeed * Input.GetAxis("Horizontal") : 0;

        movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);


        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
            isJumping = true;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
        if (characterController.isGrounded && isJumping == false)
        {
            moveDirection.y = 0;
        }

        if (characterController.isGrounded)
        {
            isJumping = false;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        // Move the controller

        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
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
