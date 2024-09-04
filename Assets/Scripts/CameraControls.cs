using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public float rotationSpeed;
    public bool canTurn;
    public float timer;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        canTurn = true;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (canTurn)
        {
            Vector3 RotDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
            orientation.forward = RotDir.normalized;

            float Horizontal = Input.GetAxis("Horizontal");
            float Vertical = Input.GetAxis("Vertical");
            Vector3 MoveDir = orientation.forward * Vertical + orientation.right * Horizontal;


            if (MoveDir != Vector3.zero)
                playerObj.forward = Vector3.Slerp(playerObj.forward, MoveDir.normalized, Time.deltaTime * rotationSpeed);
        }
    }
}
