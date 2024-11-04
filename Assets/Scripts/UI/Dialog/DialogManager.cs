using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private ChoicePanel choicePanel;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] int lettersPerSecond;

    private Dialog dialog;
    private List<string> choices;
    private Action<int> onChoiceSelected;
    private Action<int> onChoiceCancel;
    private int currentLine = 0;
    private bool isTyping;
    private bool invokeOnCloseDialog;

    public void ShowDialog(
        Dialog dialog,
        List<string> choices = null,
        Action<int> onChoiceSelected = null,
        Action<int> onChoiceCancel = null,
        bool invokeOnCloseDialog = true)
    {
        OnShowDialog?.Invoke(); // changes player state to Dialog
        this.dialog = dialog;
        this.choices = choices;
        this.onChoiceSelected = onChoiceSelected;
        this.onChoiceCancel = onChoiceCancel;
        this.invokeOnCloseDialog = invokeOnCloseDialog;
        dialogPanel.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public void ShowDialogText(
        string text,
        List<string> choices = null,
        Action<int> onChoiceSelected = null,
        Action<int> onChoiceCancel = null,
        bool invokeOnShowDialog = false,
        bool invokeOnCloseDialog = false)
    {
        if (invokeOnShowDialog)
        {
            OnShowDialog?.Invoke(); // changes player state to Dialog
        }
        dialog = new Dialog(new List<string> { text });
        this.choices = choices;
        this.onChoiceSelected = onChoiceSelected;
        this.onChoiceCancel = onChoiceCancel;
        this.invokeOnCloseDialog = invokeOnCloseDialog;
        dialogPanel.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public bool GetIsTyping()
    {
        return isTyping;
    }

    public void OnUse()
    {
        // Don't let player skip dialogue when it hasn't finished
        if (isTyping) { return; }
        currentLine++;
        if (currentLine < dialog.Lines.Count)
        {
            StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
        }
        else
        { // If there are no more lines in `dialog`
            if (choices != null && choices.Count > 1)
            {
                // If there are choices, let choicePanel handle interactions
                choicePanel.OnUse(onChoiceSelected);
            }
            else
            {
                // Otherwise, close the dialog panel
                CloseDialog();
            }
        }
    }

    public void OnCancel()
    {
        // Don't let player skip dialogue when it hasn't finished
        if (isTyping) { return; }
        currentLine++;
        if (currentLine < dialog.Lines.Count)
        {
            StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
        }
        else
        { // If there are no more lines in `dialog`
            if (choices != null && choices.Count > 1)
            {
                // If there are choices, let choicePanel handle interactions
                choicePanel.OnCancel(onChoiceCancel);
            }
            else
            {
                // Otherwise, close the dialog panel
                CloseDialog();
            }
        }
    }

    public void CloseDialog()
    {
        currentLine = 0;
        dialogPanel.SetActive(false);
        if (invokeOnCloseDialog)
        {
            OnCloseDialog?.Invoke(); // will set player state to Playing
        }
    }

    public void OnMove(Vector2 moveAmount)
    {
        if (choices != null && choices.Count > 1)
        {
            choicePanel.OnMove(moveAmount);
        }
    }

    public void HandleUpdate()
    {

    }

    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1.0f / lettersPerSecond);
        }
        isTyping = false;
        // If this is the last line and there are choices to be made,
        // display the choice panel
        if (currentLine == dialog.Lines.Count - 1
            && choices != null
            && choices.Count > 1)
        {
            choicePanel.ShowChoices(choices);
        }
    }
}
