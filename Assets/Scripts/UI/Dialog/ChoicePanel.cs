using System;
using System.Collections;
using System.Collections.Generic;
using Polybrush;
using UnityEngine;

public class ChoicePanel : MonoBehaviour
{
    [SerializeField] private ChoiceText choiceTextPrefab;

    private List<ChoiceText> choiceTexts;
    private int currentChoice;

    public void ShowChoices(List<string> choices)
    {
        currentChoice = 0;

        gameObject.SetActive(true);

        // Delete existing children
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Add choices as children
        choiceTexts = new List<ChoiceText>();
        foreach (string choice in choices)
        {
            ChoiceText choiceTextObj = Instantiate(choiceTextPrefab, transform);
            choiceTextObj.TextField.text = choice;
            choiceTexts.Add(choiceTextObj);
        }
        UpdateSelected();
    }

    private void UpdateSelected()
    {
        for (int i = 0; i < choiceTexts.Count; i++)
        {
            choiceTexts[i].SetSelected(i == currentChoice);
        }
    }

    public void OnMove(Vector2 moveAmount)
    {
        if (moveAmount.y < 0)
        {
            // Down
            currentChoice++;
        }
        else if (moveAmount.y > 0)
        {
            // Up
            currentChoice--;
        }

        currentChoice = Mathf.Clamp(currentChoice, 0, choiceTexts.Count - 1);
        UpdateSelected();
    }

    public void OnUse(Action<int> onChoiceSelected)
    {
        onChoiceSelected?.Invoke(currentChoice);
        gameObject.SetActive(false);
    }

    public void OnCancel(Action<int> onChoiceCancel)
    {
        onChoiceCancel?.Invoke(currentChoice);
        gameObject.SetActive(false);
    }
}
