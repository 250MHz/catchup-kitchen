using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotPanManager : MonoBehaviour
{
    public static PotPanManager Instance { get; private set; }

    [SerializeField] private KitchenStove[] stovesToSpawnObjectsOn;
    [SerializeField] private UsableObjectSO potSO;
    [SerializeField] private UsableObjectSO panSO;
    [SerializeField] private int maxPotCount = 2;
    [SerializeField] private int maxPanCount = 2;

    private int potCount = 0;
    private int panCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < stovesToSpawnObjectsOn.Length; i++)
        {
            if (i % 2 == 0 && TryIncreasePotCount())
            {
                UsableObject pot = Instantiate(
                        potSO.GetPrefab(),
                        new Vector3(0, 0, 0),
                        Quaternion.identity
                    ).GetComponent<UsableObject>();
                pot.SetUsableObjectParent(stovesToSpawnObjectsOn[i]);
            }
            else if (i % 2 == 1 && TryIncreasePanCount())
            {
                UsableObject pan = Instantiate(
                        panSO.GetPrefab(),
                        new Vector3(0, 0, 0),
                        Quaternion.identity
                    ).GetComponent<UsableObject>();
                pan.SetUsableObjectParent(stovesToSpawnObjectsOn[i]);
            }
        }
    }

    public bool TryIncreasePotCount()
    {
        if (potCount < maxPotCount)
        {
            potCount++;
            return true;
        }
        return false;
    }

    public bool TryIncreasePanCount()
    {
        if (panCount < maxPanCount)
        {
            panCount++;
            return true;
        }
        return false;
    }

    public bool MaxPotCountReached()
    {
        return potCount >= maxPotCount;
    }

    public bool MaxPanCountReached()
    {
        return panCount >= maxPanCount;
    }
}
