using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnFood : MonoBehaviour
{
    public int Berries;
    public int Meat;
    public int Rubbish;
    public int total;
    public int NewTot;

    public GameObject Food;
    public GameObject Berry;
    public GameObject Meats;

    public GameObject BerryStorage;
    public GameObject MeatStorage;
    public GameObject RubbishStorage;

    public Transform[] BinLocations;
    public bool[] BinLocationTaken;
    public Transform[] CityTreeLocations;
    public bool[] CityTreeLocationsTaken;
    public Transform[] ForestLocations;
    public bool[] ForestLocationsTaken;

    public string[] FoodTypeArray;

    public GameObject Bins;
    public GameObject CityTrees;
    public GameObject Forest;
    public GameObject Fox;

    public Text BerriesText;
    public Text MeatText;
    public Text RubbishText;
    // Start is called before the first frame update
    void Start()
    {
        BinLocations = Bins.gameObject.GetComponentsInChildren<Transform>();
        for(int i = 0; i < BinLocations.Length; i++)
        {
            BinLocationTaken[i] = false;
        }
        CityTreeLocations = CityTrees.gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < CityTreeLocations.Length; i++)
        {
            CityTreeLocationsTaken[i] = false;
        }
        ForestLocations = Forest.gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < ForestLocations.Length; i++)
        {
            ForestLocationsTaken[i] = false;
        }

        FoodTypes();
        Fox.GetComponentInParent<Movement>().LeftToCollect[0] = Berries;
        Fox.GetComponentInParent<Movement>().LeftToCollect[1] = Meat;
        Fox.GetComponentInParent<Movement>().LeftToCollect[2] = Rubbish;

        for (int i = 0; i < NewTot; i++)
        {
            float X = Random.Range(-125, 125);
            float Z = Random.Range(-125, 125);
            Vector3 Pos = new Vector3(X, 0.5f, Z);
            if(FoodTypeArray[i] == "Berry")
            {
                while (true)
                {
                    int J = Random.Range(0, CityTreeLocations.Length);
                    if (CityTreeLocationsTaken[J] == false)
                    {
                        int x= Random.Range(-15, 15);
                        int z = Random.Range(-15, 15);
                        Vector3 TreePos = new Vector3(CityTreeLocations[J].position.x + x, 2f, CityTreeLocations[J].position.z + z);
                        Instantiate(Berry, TreePos, Quaternion.Euler(-90, 0, 0), BerryStorage.transform);
                        CityTreeLocationsTaken[J] = true;
                        break;
                    }
                }
            }
            if (FoodTypeArray[i] == "Meat")
            {
                while (true)
                {
                    int J = Random.Range(0, ForestLocations.Length);
                    if (ForestLocationsTaken[J] == false)
                    {
                        int x = Random.Range(-15, 15);
                        int z = Random.Range(-15, 15);
                        Vector3 TreePos = new Vector3(ForestLocations[J].position.x + x, 0f, ForestLocations[J].position.z + z);
                        Instantiate(Meats, TreePos, Quaternion.identity, MeatStorage.transform);
                        ForestLocationsTaken[J] = true;
                        break;
                    }
                }
            }
            if (FoodTypeArray[i] == "Rubbish")
            {
                while (true)
                {
                    int J = Random.Range(0, BinLocations.Length);
                    if (BinLocationTaken[J] == false)
                    {
                        Vector3 BinPos = new Vector3(BinLocations[J].position.x,0.5f, BinLocations[J].position.z);
                        Instantiate(Food, BinPos, Quaternion.identity, RubbishStorage.transform);
                        BinLocationTaken[J] = true;
                        break;
                    }
                }
            }
            
        }
        UpdateUI();
    }
    public void UpdateUI()
    {
        BerriesText.text = Berries.ToString();
        MeatText.text = Meat.ToString();
        RubbishText.text = Rubbish.ToString();
    }
    void FoodTypes()
    {
        total = Random.Range(30, FoodTypeArray.Length);
        int Num = total / 3;
        Berries = Num + 8;
        Meat = Num - 2;
        Rubbish = Num -6;
        NewTot = Berries + Meat + Rubbish;
        for(int i = 0; i < total; i++)
        {
            if (i < Berries)
            {
                FoodTypeArray[i] = "Berry";
            }
            if (i < Berries + Meat && i >= Berries)
            {
                FoodTypeArray[i] = "Meat";
            }
            if (i < Berries + Meat + Rubbish && i >= Berries + Meat && i >= Berries)
            {
                FoodTypeArray[i] = "Rubbish";
            }
        }
    }
}
