using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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
    bool StopJump;

    [Header("Slope Controls")]
    public float maxSlopeAngle;
    public RaycastHit slopeHit;
    bool exitingSlope;

    [Header("Grounding Controls")]
    public float groundDrag;
    public float playerHeight;
    public LayerMask Ground;
    bool grounded;

    Vector3 moveDir;

    Rigidbody rb;
    public Transform orientation;
    RaycastHit wallCheckHit;

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
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.25f, Ground);
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
        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0 && !readyToJump)
            StopJump = true;
        if (Input.GetKeyUp(KeyCode.LeftShift) && MoveMult == 2 || Input.GetKeyUp(KeyCode.LeftControl) && MoveMult != 2 || !grounded)
            MoveMult = 1;
        if (Input.GetKey(KeyCode.LeftShift) && MoveMult == 1 && Stamina > 15f && grounded)
            MoveMult *= 2;
        if (Input.GetKey(KeyCode.LeftControl) && MoveMult == 1 && grounded)
            MoveMult /= 3;

        if(StopJump)
        {
            rb.velocity = new(rb.velocity.x, rb.velocity.y - 10 * Time.deltaTime, rb.velocity.z);
            if(rb.velocity.y <=0)
            {
                StopJump = false;
            }
        }
        //Caps the players speed if they exceed it
        if (OnSlope() && !exitingSlope)
            if(rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        
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

        rb.useGravity = !OnSlope();
        if (OnSlope() && !exitingSlope)
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            if (angle >= maxSlopeAngle / 2 && rb.velocity.y > 0)
            {
                if (angle / maxSlopeAngle >= 1)
                    MoveMult = 0.1f;
                else
                    MoveMult = angle / maxSlopeAngle;
            }
            else
                MoveMult = 1;
            if (rb.velocity.y < 0)
                rb.AddForce(20f * MoveMult * moveSpeed * Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized, ForceMode.Force);
            if (rb.velocity.y > 0)
            {
                rb.AddForce(20f * MoveMult * moveSpeed * Vector3.ProjectOnPlane(moveDir, -slopeHit.normal).normalized, ForceMode.Force);
                rb.AddForce(Vector3.down * 120f, ForceMode.Force);
            }
        }
        else if (grounded)
        {
            rb.AddForce(10f * MoveMult * moveSpeed * moveDir.normalized, ForceMode.Force);
            MoveMult = 1;
        }
        else if (!grounded)
        {
            rb.AddForce(10f * airMultiplier * MoveMult * moveSpeed * moveDir.normalized, ForceMode.Force);
            MoveMult = 1;
        }
        if (Physics.Raycast(orientation.position, orientation.forward, out wallCheckHit, 3.5f))
        {
            if (rb.velocity.y > 0 && Vector3.Angle(Vector3.up, wallCheckHit.transform.position) > 87 && wallCheckHit.transform.gameObject.layer == 7)
            {
                float drag = (-1f - ((Vector3.Angle(Vector3.up, wallCheckHit.transform.position) - 88) * 3f));
                Debug.Log(drag);
                if (drag < -2f)
                    drag -= 0.75f;
                rb.velocity = new(rb.velocity.x, drag, rb.velocity.z);
            }
        }
    }

    void Jump()
    {
        exitingSlope = true;
        rb.velocity = new(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    void ResetJump()
    {
        exitingSlope = false;
        readyToJump = true;
    }
    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 1.25f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle != 0;
        }
        return false;
    }
}
