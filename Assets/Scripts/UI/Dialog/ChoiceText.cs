using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ChoiceText : MonoBehaviour
{
    [SerializeField] private Image selectedBorder;

    private TextMeshProUGUI text;
    private RectTransform borderRt;
    private RectTransform textRt;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        borderRt = selectedBorder.GetComponent<RectTransform>();
        textRt = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (borderRt.rect.width != textRt.rect.width)
        {
            Util.MatchRectTransformSize(borderRt, textRt, textRt.rect.width, textRt.rect.height);
        }
    }

    public void SetSelected(bool selected)
    {
        selectedBorder.gameObject.SetActive(selected);
    }

    public TextMeshProUGUI TextField => text;
}
