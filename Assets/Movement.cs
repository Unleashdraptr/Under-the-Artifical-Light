using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Speed Controls")]
    public float moveSpeed;
    public float MoveMult;

    [Header("Jump Controls")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Grounding Controls")]
    public float groundDrag;
    public float playerHeight;
    public LayerMask Ground;
    bool grounded;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDir;

    Rigidbody rb;
    public Transform orientation;
    // Start is called before the first frame update
    void Start()
    {
        readyToJump = true;
        rb = GetComponent<Rigidbody>();   
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Gathering of Inputs
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        //Ground Check and Drag applied if so
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        //Check if the player is eligible to Jump
        if (Input.GetKey(KeyCode.Space) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
        if(Input.GetKeyUp(KeyCode.Space))
            rb.velocity = new(rb.velocity.x, 1.5f, rb.velocity.z);

        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftControl))
            MoveMult = 1;
        if (Input.GetKeyDown(KeyCode.LeftShift) && MoveMult == 1)
            MoveMult *= 2;
        if (Input.GetKeyDown(KeyCode.LeftControl) && MoveMult == 1)
            MoveMult /= 3;

        //Caps the players speed if they exceed it
        Vector3 flatVel = new(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed * MoveMult)
        {
            Vector3 limitedVel = MoveMult * moveSpeed * flatVel.normalized;
            rb.velocity = new(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    private void FixedUpdate()
    {
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
            rb.AddForce(10f * MoveMult * moveSpeed * moveDir.normalized, ForceMode.Force);

        if (!grounded)
            rb.AddForce(10f * airMultiplier * MoveMult * moveSpeed * moveDir.normalized, ForceMode.Force);
    }

    void Jump()
    {
        rb.velocity = new(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    void ResetJump()
    {
        readyToJump = true;
    }
}
