using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShadowControl : MonoBehaviour
{
    RaycastHit hit;
    // Update is called once per frame
    void Update()
    {
        if (transform.GetComponentInParent<Movement>().MoveState == Movement.AnimControls.FALLING)
        {
            Physics.Raycast(transform.parent.position, Vector3.down, out hit);
            transform.position = new(transform.position.x, hit.transform.position.y + 0.25f + hit.transform.lossyScale.y, transform.position.z);
        }
        else
            transform.position = new(transform.position.x, transform.parent.position.y - 10f, transform.position.z);
    }
}
