using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Speed Controls")]
    public float moveSpeed;
    public float MoveMult;
    public float Stamina;

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
        if(Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0 && !readyToJump)
            rb.velocity = new(rb.velocity.x, rb.velocity.y/4, rb.velocity.z);
        if (Input.GetKeyUp(KeyCode.LeftShift) && MoveMult == 2 || Input.GetKeyUp(KeyCode.LeftControl) && MoveMult != 2)
            MoveMult = 1;
        if (Input.GetKey(KeyCode.LeftShift) && MoveMult == 1 && Stamina > 15f)
            MoveMult *= 2;
        if (Input.GetKey(KeyCode.LeftControl) && MoveMult == 1)
            MoveMult /= 3;

        //Caps the players speed if they exceed it
        Vector3 flatVel = new(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed * MoveMult)
        {
            Vector3 limitedVel = MoveMult * moveSpeed * flatVel.normalized;
            rb.velocity = new(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

        if(MoveMult > 1)
        {
            Stamina -= Time.deltaTime;
            if (Stamina <= 0)
                MoveMult = 1;
        }
        if (Stamina < 25 && MoveMult < 2)
        {
            Stamina += Time.deltaTime;
            if(Stamina > 25)
                Stamina = 25;
        }
    }
    private void FixedUpdate()
    {
        moveDir = orientation.forward * Input.GetAxis("Vertical") + orientation.right * Input.GetAxis("Horizontal");

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
