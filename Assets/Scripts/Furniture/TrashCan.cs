using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour, IInteractable
{
    [SerializeField] private UsableObjectSO plateUsableObjectSO;
    [SerializeField] private UsableObjectSO potUsableObjectSO;
    [SerializeField] private UsableObjectSO dirtyPlateUsableObjectSO;
    private Outline outline;

    public void Interact(Player player)
    {
        if (player.HasUsableObject())
        {
            UsableObject usableObject = player.GetUsableObject();
            if (usableObject.GetUsableObjectSO() == dirtyPlateUsableObjectSO)
            {
                // Debug.Log("Cannot interact with the trash can while holding a dirty plate.");
                return; // Prevent interaction if holding a dirty plate
            }

            player.ClearUsableObject();
            Destroy(usableObject.gameObject);
            if (usableObject is PlateUsableObject)
            {
                // Replace what player is holding with empty plate
                UsableObject.SpawnUsableObject(plateUsableObjectSO, player);
            }
            else if (usableObject is PotUsableObject)
            {
                // Replace what palyer is holding with empty pot
                UsableObject.SpawnUsableObject(potUsableObjectSO, player);
            }
        }
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        outline = gameObject.GetComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
