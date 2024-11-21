using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI upText;
    [SerializeField] private TextMeshProUGUI downText;
    [SerializeField] private TextMeshProUGUI leftText;
    [SerializeField] private TextMeshProUGUI rightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI cancelText;

    private Dictionary<string, string> keyValuePairs;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        keyValuePairs = new Dictionary<string, string>
        {
            { "Up", "↑" },
            { "Down", "↓" },
            { "Left", "←"},
            { "Right", "→"},
            { "Num 1", "1"},
            { "Num 2", "2"},
        };
    }

    public void SetKeyText(string scheme, PlayerInput playerInput)
    {
        string move = Util.GetBindingDisplayStringOrCompositeName(scheme, playerInput.actions["Move"]);
        string[] moves = move.Split("/");
        upText.text = keyValuePairs.GetValueOrDefault(moves[0], moves[0]);
        leftText.text = keyValuePairs.GetValueOrDefault(moves[1], moves[1]);
        downText.text = keyValuePairs.GetValueOrDefault(moves[2], moves[2]);
        rightText.text = keyValuePairs.GetValueOrDefault(moves[3], moves[3]);
        interactText.text = keyValuePairs.GetValueOrDefault(
            Util.GetBindingDisplayStringOrCompositeName(scheme, playerInput.actions["Use"]),
            Util.GetBindingDisplayStringOrCompositeName(scheme, playerInput.actions["Use"])
        );
        cancelText.text = keyValuePairs.GetValueOrDefault(
            Util.GetBindingDisplayStringOrCompositeName(scheme, playerInput.actions["Cancel"]),
            Util.GetBindingDisplayStringOrCompositeName(scheme, playerInput.actions["Cancel"])
        );
    }

    public void ShowTemporarily()
    {
        canvasGroup.alpha = 1f;
        StartCoroutine(WaitForSeconds(10f));
    }

    private IEnumerator WaitForSeconds(float sec)
    {
        yield return new WaitForSeconds(sec);
        StartCoroutine(Fade(5f));
    }

    private IEnumerator Fade(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - (elapsed / duration);
            yield return null;
        }
    }
}
