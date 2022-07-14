using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public bool HasItem;
    void Start()
    {
       int HasItemChance = Random.Range(1, 10);

        if(HasItemChance == 9)
        {
            HasItem = true;
        }
    }

    void Update()
    {
        
    }
}
