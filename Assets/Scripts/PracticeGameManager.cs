using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeGameManager : MonoBehaviour
{

    [SerializeField] private KitchenCabinet cabinetForBurnedPot;
    [SerializeField] private UsableObjectSO tomatoBurnedSO;
    [SerializeField] private KitchenCabinet cabinetForDirtyPlate;
    [SerializeField] private UsableObjectSO dirtyPlateSO;

    private void Start()
    {
        UsableObject burnedPot = Instantiate(
                tomatoBurnedSO.GetPrefab(),
                new Vector3(0, 10, 0),
                Quaternion.identity
            ).GetComponent<UsableObject>();
        burnedPot.SetUsableObjectParent(cabinetForBurnedPot);
        UsableObject dirtyPlate = Instantiate(
                dirtyPlateSO.GetPrefab(),
                new Vector3(0, 0, 0),
                Quaternion.identity
            ).GetComponent<UsableObject>();
        dirtyPlate.SetUsableObjectParent(cabinetForDirtyPlate);

    }
}
