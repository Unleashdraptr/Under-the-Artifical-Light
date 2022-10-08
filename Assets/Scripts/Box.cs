using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public bool HasItem;
    public string FoodType;
    public Renderer Mymaterial;
    void Start()
    {
       int HasItemChance = Random.Range(1, 10);

        if (HasItemChance >= 7)
        {
            HasItem = true;
        }
        else
            HasItem = false;
        if(HasItem == true)
        {
            if(FoodType == "Berry")
            {
                Mymaterial.material.color = Color.green;
            }
            if (FoodType == "Meat")
            {
                Mymaterial.material.color = Color.red;
            }
            if (FoodType == "Rubbish")
            {
                Mymaterial.material.color = Color.grey;
            }
            if (FoodType == "Water")
            {
                Mymaterial.material.color = Color.blue;
            }
        }
    }
}
