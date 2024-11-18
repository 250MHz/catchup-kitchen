using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HelpOrb : MonoBehaviour, IInteractable
{
    [SerializeField] private string subjectName;
    [SerializeField] private Dialog dialog;

    private Outline outline;

    public void Interact(Player player)
    {
        DialogManager dialogManager = player.GetDialogManager();
        dialogManager.ShowDialogText(
            $"Would you like to read the help instructions for {subjectName}?",
            choices: new List<string>() { "Yes", "No" },
            onChoiceSelected: (choiceIndex) =>
            {
                if (choiceIndex == 0)
                {
                    dialogManager.ShowDialog(dialog);
                }
                else
                {
                    dialogManager.CloseDialog();
                }
            },
            onChoiceCancel: (choiceIndex) =>
            {
                dialogManager.CloseDialog();
            },
            invokeOnShowDialog: true,
            invokeOnCloseDialog: true
        );
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    private void Start()
    {
        outline = gameObject.GetComponent<Outline>();
    }
}
