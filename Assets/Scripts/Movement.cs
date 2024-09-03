using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{
    [Header("Speed Controls")]
    public float moveSpeed;
    public float MoveMult;
    public float[] StateSpeeds;
    public float Stamina;
    Vector3 moveForce;

    [Header("Jump Controls")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool jumpStopping;
    bool readyToJump;

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
    Vector3 lastMoveDir;
    int FrameCount;
    Vector3 MoveDirections;
    Vector3 PrevMoveDirections;

    Rigidbody rb;
    public Transform orientation;
    public CameraControls TurnLock;
    public float timer;
    RaycastHit wallCheckHit;

    public enum AnimControls {IDLE, WALKING, SPRINTING, CROUCHING, SKIDDING, JUMPING, FALLING }
    public AnimControls MoveState;

    // Start is called before the first frame update
    void Start()
    {
        MoveState = AnimControls.IDLE;
        readyToJump = true;
        rb = GetComponent<Rigidbody>();   
        rb.freezeRotation = true;
        TurnLock = GameObject.Find("Camera").GetComponent<CameraControls>();
    }

    // Update is called once per frame
    void Update()
    {
        //Ground Check and Drag applied if so
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.25f, Ground);
        if (grounded)
        {
            rb.drag = groundDrag;
            jumpStopping = false;
        }
        else
            rb.drag = 0;


        if (MoveState != AnimControls.SKIDDING)
        {
            //Jump Controls
            if (Input.GetKey(KeyCode.Space) && readyToJump && grounded)
            {
                Jump();
                jumpStopping = false;
                Invoke(nameof(StopJump), jumpCooldown);
            }
            if (Input.GetKeyUp(KeyCode.Space) || (!grounded && MoveState != AnimControls.JUMPING))
                jumpStopping = true;
            if (jumpStopping)
                StopJump();



            //Grounded controls and checks for other movement states such as Sprinting
            if ((MoveState != AnimControls.JUMPING) && grounded)
            {
                //Different Speed Options Avialable to the player
                MoveState = AnimControls.WALKING;
                if (Input.GetKey(KeyCode.LeftShift) && Stamina > 15f)
                    MoveState = AnimControls.SPRINTING;
                else if (Input.GetKey(KeyCode.LeftControl))
                    MoveState = AnimControls.CROUCHING;


                if (MoveState != AnimControls.IDLE && (int)MoveState < 4)
                    MoveMult = StateSpeeds[(int)MoveState - 1];
                if (MoveState == AnimControls.SPRINTING)
                {
                    Stamina -= Time.deltaTime;
                    if (Stamina <= 0)
                        MoveMult = 1;
                }
                if (Stamina < 25 && MoveState != AnimControls.SPRINTING)
                {
                    Stamina += Time.deltaTime;
                    if (Stamina > 25)
                        Stamina = 25;
                }
                if (rb.velocity.x <= 0.5f && rb.velocity.z <= 0.5f && rb.velocity.x >= -0.5f && rb.velocity.z >= -0.5f)
                    MoveState = AnimControls.IDLE;
            }
        }




        //Caps the players speed if they exceed it, even works on slops
        if (OnSlope() && !exitingSlope)
            if(rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        
        Vector3 flatVel = new(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed * MoveMult)
        {
            Vector3 limitedVel = MoveMult * moveSpeed * flatVel.normalized;
            rb.velocity = new(limitedVel.x, rb.velocity.y, limitedVel.z);
        }




        if (MoveState != AnimControls.IDLE && PrevMoveDirections != Vector3.zero)
        {
            if ((Input.GetAxis("Vertical") == 0 || Input.GetAxis("Horizontal") == 0) && Input.GetAxis("Mouse X") <= 0.5f && MoveState == AnimControls.SPRINTING)
            {
                if (moveDir.z < 0)
                    moveDir.z *= -1;
                if (moveDir.x < 0)
                    moveDir.x *= -1;
                if ((MoveDirections.x != PrevMoveDirections.x && lastMoveDir.x - moveDir.x <= 0.15f) || (MoveDirections.z != PrevMoveDirections.z && lastMoveDir.z - moveDir.z <= 0.15f))
                {
                    timer = 0.35f;
                    MoveState = AnimControls.SKIDDING;
                    TurnLock.canTurn = false;
                }
            }
        }
        if (MoveState == AnimControls.SKIDDING)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                MoveState = AnimControls.WALKING;
                TurnLock.canTurn = true;
            }
        }
        if (FrameCount == 15)
        {
            lastMoveDir = moveDir;
            PrevMoveDirections = MoveDirections;
            if (lastMoveDir.z < 0)
                lastMoveDir.z *= -1;
            if (lastMoveDir.x < 0)
                lastMoveDir.x *= -1;
            FrameCount = 0;
        }
        else
            FrameCount++;
    }





    private void FixedUpdate()
    {
        moveDir = orientation.forward * Input.GetAxis("Vertical") + orientation.right * Input.GetAxis("Horizontal");
        moveDir = moveDir.normalized;
        MoveDirections = new(Mathf.Round(Input.GetAxis("Vertical")), 0, Mathf.Round(Input.GetAxis("Horizontal")));
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
                moveForce = (20f * MoveMult * moveSpeed * Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized);
            if (rb.velocity.y > 0)
            {
                moveForce = 20f * MoveMult * moveSpeed * Vector3.ProjectOnPlane(moveDir, -slopeHit.normal).normalized;
                moveForce += Vector3.down * 120f;
            }
        }
        else if (grounded)
        {
            moveForce = 10f * MoveMult * moveSpeed * moveDir;
            MoveMult = 1;
        }
        else if (!grounded)
        {
            moveForce = 10f * airMultiplier * MoveMult * moveSpeed * moveDir;
            MoveMult = 1;
        }
        if(MoveState == AnimControls.SKIDDING)
        {
            moveForce.x *= 0.25f;
            moveForce.z *= 0.25f;
        }

        rb.AddForce(moveForce, ForceMode.Force);

        //If hitting a steep Slope
        if (Physics.Raycast(orientation.position, orientation.forward, out wallCheckHit, 3.5f))
        {
            if (rb.velocity.y > 0 && Vector3.Angle(Vector3.up, wallCheckHit.transform.position) > 87 && wallCheckHit.transform.gameObject.layer == 7)
            {
                float drag = (-1f - ((Vector3.Angle(Vector3.up, wallCheckHit.transform.position) - 88) * 3f));
                if (drag < -2f)
                    drag -= 0.75f;
                rb.velocity = new(rb.velocity.x, drag, rb.velocity.z);
            }
        }
    }

    void Jump()
    {
        MoveMult = 1;
        readyToJump = false;
        exitingSlope = true;
        rb.velocity = new(rb.velocity.x, 0f, rb.velocity.z);
        MoveState = AnimControls.JUMPING;
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void StopJump()
    {
        jumpStopping = true;
        rb.velocity = new(rb.velocity.x, rb.velocity.y - 10 * Time.deltaTime, rb.velocity.z);
        if (rb.velocity.y <= 0)
        {
            exitingSlope = false;
            readyToJump = true;
            MoveState = AnimControls.FALLING;
        }
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
