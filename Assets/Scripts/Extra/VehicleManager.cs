using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    [SerializeField] private Transform spawnLocation;
    [SerializeField] private Transform endLocation;
    [SerializeField] private GameObject[] vehiclePrefabs;
    [SerializeField] private float spawnTime;

    private float elapsedTime;
    private float lastSpawnTime;
    private GameObject vehicle;

    private void Start()
    {
        elapsedTime = 0f;
        lastSpawnTime = 0f;
        vehicle = null;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime - lastSpawnTime >= spawnTime)
        {
            vehicle = Instantiate(vehiclePrefabs[Random.Range(0, vehiclePrefabs.Length - 1)], spawnLocation);
            lastSpawnTime = elapsedTime;
        }
        if (vehicle != null)
        {
            if (vehicle.transform.position.z < endLocation.position.z)
            {
                vehicle.transform.Translate(Vector3.forward * Time.deltaTime * 15);
            }
            else
            {
                Destroy(vehicle);
                vehicle = null;
            }
        }
    }
}
