using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    void Update()
    {
        this.transform.Rotate(0, 0, 0.5f);
    }
}
