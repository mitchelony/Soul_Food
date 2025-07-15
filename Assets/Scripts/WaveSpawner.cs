using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public List<Customer> Customers = new List<Customer>();
    public int currWave = 1;
    public int waveValue;
    public List<GameObject> customersToSpawn = new List<GameObject>();

    public int CurrWave { get => currWave; set => currWave = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateWave();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void GenerateWave()
    {
        waveValue = CurrWave * 10;
        GenerateCustomers();
    }

    public void GenerateCustomers()
    {
        List<GameObject> generatedCustomers = new List<GameObject>();
        while (waveValue > 0)
        {
            int randCustomerId = Random.Range(0, Customers.Count);
            int randCustomerCost = Customers[randCustomerId].cost;

            if (waveValue - randCustomerCost >= 0)
            {
                generatedCustomers.Add(Customers[randCustomerId].CustomerPrefab);
                waveValue -= randCustomerCost;
            }
            else if (waveValue <= 0)
            {
                break;
            }
        }
        customersToSpawn.Clear();
        customersToSpawn = generatedCustomers;
    }
}

[System.Serializable]
public class Customer
{
    public GameObject CustomerPrefab;
    public int cost;
}