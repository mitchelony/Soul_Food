using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public List<Customer> Customers = new List<Customer>();
    public int currWave = 1;
    private int waveValue;
    public List<GameObject> customersToSpawn = new List<GameObject>();

    public Transform spawnLocation;
    public float customerSpacing = 3f; // Distance between each customer (bigger than their size)
    public int waveDuration;
    private float waveTimer;
    private float spawnInterval;
    private float spawnTimer;
    private int totalCustomersInWave; // Track total customers for positioning
    // public int CurrWave { get => currWave; set => currWave = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateWave();
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            if (customersToSpawn.Count > 0)
            {
                // Calculate predetermined position based on how many customers we've spawned
                int spawnedCount = totalCustomersInWave - customersToSpawn.Count;
                Vector3 spawnPos = GetSpawnPosition(spawnedCount);

                Instantiate(customersToSpawn[0], spawnPos, Quaternion.identity);
                customersToSpawn.RemoveAt(0);
                spawnTimer = spawnInterval;
            }
            else
            {
                waveTimer = 0;
            }
        }
    }
    public void GenerateWave()
    {
        waveValue = currWave * 10;
        GenerateCustomers();

        if (customersToSpawn.Count > 0)
        {
            spawnInterval = (float)waveDuration / customersToSpawn.Count;
            spawnTimer = spawnInterval;
        }
        waveTimer = waveDuration;
    }

    public void GenerateCustomers()
    {
        List<GameObject> generatedCustomers = new List<GameObject>();
        int maxAttempts = 100; // Safety limit to prevent infinite loops
        int attempts = 0;

        while (waveValue > 0 && attempts < maxAttempts)
        {
            attempts++;
            int randCustomerId = Random.Range(0, Customers.Count);
            int randCustomerCost = Customers[randCustomerId].cost;

            if (waveValue - randCustomerCost >= 0)
            {
                generatedCustomers.Add(Customers[randCustomerId].CustomerPrefab);
                waveValue -= randCustomerCost;
            }
            else
            {
                // Can't afford any more customers, exit the loop
                bool canAffordAny = false;
                for (int i = 0; i < Customers.Count; i++)
                {
                    if (Customers[i].cost <= waveValue)
                    {
                        canAffordAny = true;
                        break;
                    }
                }
                if (!canAffordAny)
                {
                    break; // Exit if we can't afford any customer
                }
            }
        }

        customersToSpawn.Clear();
        customersToSpawn = generatedCustomers;
        totalCustomersInWave = generatedCustomers.Count; // Store total count

        Debug.Log($"Generated {generatedCustomers.Count} customers for wave {currWave} with {waveValue} points remaining");
    }

    private Vector3 GetSpawnPosition(int customerIndex)
    {
        // Create a grid pattern with 3 customers per row
        int customersPerRow = 3;
        int row = customerIndex / customersPerRow;
        int col = customerIndex % customersPerRow;

        // Calculate offset from spawn location
        // Center the row horizontally (subtract 1 to center around middle position)
        float xOffset = (col - 1) * customerSpacing; // -1, 0, 1 for 3 customers
        float yOffset = row * customerSpacing; // 0, 1, 2... for each row going up

        return spawnLocation.position + new Vector3(xOffset, yOffset, 0);
    }

}

[System.Serializable]
public class Customer
{
    public GameObject CustomerPrefab;
    public int cost;
}