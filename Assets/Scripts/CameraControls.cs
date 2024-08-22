using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public float MoveSpeed;
    public float JumpHeight;

    public Transform Orientation;
    public Transform Player;
    public Transform PlayerObj;
    public float RotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 RotDir = Player.position - new Vector3(transform.position.x, Player.position.y, transform.position.z);
        Orientation.forward = RotDir.normalized;

        float Horizontal = Input.GetAxis("Horizontal");
        float Vertical = Input.GetAxis("Vertical");
        Vector3 MoveDir = Orientation.forward * Vertical + Orientation.right * Horizontal;

        if (MoveDir != Vector3.zero)
            PlayerObj.forward = Vector3.Slerp(PlayerObj.forward, MoveDir.normalized, Time.deltaTime * RotationSpeed);
    }
}
