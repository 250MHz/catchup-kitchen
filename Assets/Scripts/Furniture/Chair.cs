using UnityEngine;

public class Chair : BaseFurniture
{
    [SerializeField] private Transform seatPoint;
    // BaseFurniture.topPoint is used in GlassTable to know where to
    // place a plate

    private bool isOccupied = false;
    private NPCController occupyingNPC;
    private Outline outline;


    // public void Interact(Player player)
    // {
    //     Debug.Log("Chair Interact() called");
    // }


    // public void EnableOutline()
    // {
    //     if (outline != null)
    //     {
    //         outline.enabled = true;
    //     }
    // }

    // public void DisableOutline()
    // {
    //     if (outline != null)
    //     {
    //         outline.enabled = false;
    //     }
    // }


    // Start is called before the first frame update
    private void Start()
    {
        outline = gameObject.GetComponentInChildren<Outline>();
    }


    // Update is called once per frame
    void Update()
    {

    }

    public Transform GetSeatPoint()
    {
        return seatPoint;
    }
}
