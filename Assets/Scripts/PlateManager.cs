using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlateManager : MonoBehaviour
{
    public static PlateManager Instance { get; private set; }

    [SerializeField] private KitchenCabinet[] cabinetsToSpawnPlatesOn;
    [SerializeField] private UsableObjectSO plateSO;
    [SerializeField] private int maxPlateCount = 16;
    private int plateCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (KitchenCabinet kc in cabinetsToSpawnPlatesOn)
        {
            if (TryIncreasePlateCount())
            {
                UsableObject plate = Instantiate(
                        plateSO.GetPrefab(),
                        new Vector3(0, 0, 0),
                        Quaternion.identity
                    ).GetComponent<UsableObject>();
                plate.SetUsableObjectParent(kc);
            }
        }
    }

    public bool TryIncreasePlateCount()
    {
        if (plateCount < maxPlateCount)
        {
            plateCount++;
            return true;
        }
        return false;
    }

    public bool MaxPlateCountReached()
    {
        return plateCount >= maxPlateCount;
    }
}
