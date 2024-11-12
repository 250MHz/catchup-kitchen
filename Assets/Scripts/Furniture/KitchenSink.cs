using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenSink : BaseFurniture, IInteractable
{
    [SerializeField] private UsableObjectSO plateDirtySO; // input
    [SerializeField] private UsableObjectSO plateSO; // output
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private int cleaningProgressMax = 6;

    private Outline outline;
    private int cleaningProgress;

    private AudioSource waterAudioSource;
    private AudioSource plateCleaningAudioSource;
    private bool isWashing = false;

    [SerializeField] private ParticleSystem waterFlowEffect;

    public void Interact(Player player)
    {
        if (!HasUsableObject())
        {
            // Sink is empty
            if (player.HasUsableObject())
            {
                // If the object the player is holding is a dirty plate, then
                // make the sink the owner of the dirty plate
                UsableObject heldObject = player.GetUsableObject();
                if (heldObject.GetUsableObjectSO() == plateDirtySO)
                {
                    heldObject.SetUsableObjectParent(this);
                    cleaningProgress = 0;
                    UpdateProgressBar();
                    StartWashing();
                }
            }
        }
        else
        {
            // Sink has a dirty plate in it
            if (!player.HasUsableObject())
            {
                // Player is not holding anything
                UsableObject currentUsableObject = GetUsableObject();
                // If plate is dirty, clean it
                if (currentUsableObject.GetUsableObjectSO() == plateDirtySO)
                {
                    cleaningProgress++;
                    UpdateProgressBar();

                    plateCleaningAudioSource.Play();

                    if (cleaningProgress >= cleaningProgressMax)
                    {
                        currentUsableObject.DestroySelf();
                        // Once clean, make the player hold a clean plate
                        UsableObject.SpawnUsableObject(plateSO, player);

                        StopWashing();
                    }
                }
            }
        }
    }

    private void UpdateProgressBar()
    {
        progressBar.SetBarFillAmount((float)cleaningProgress / cleaningProgressMax);
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    private void StartWashing()
    {
        if (!isWashing)
        {
            isWashing = true;
            waterAudioSource.Play();

            if (!waterFlowEffect.isPlaying)
            {
                waterFlowEffect.Play();
            }
        }
    }

    private void StopWashing()
    {
        if (isWashing)
        {
            isWashing = false;
            waterAudioSource.Stop();

            if (waterFlowEffect.isPlaying)
            {
                waterFlowEffect.Stop();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        outline = gameObject.GetComponent<Outline>();
        waterAudioSource = gameObject.GetComponents<AudioSource>()[0];
        plateCleaningAudioSource = gameObject.GetComponents<AudioSource>()[1];
    }

    // Update is called once per frame
    void Update()
    {

    }
}
