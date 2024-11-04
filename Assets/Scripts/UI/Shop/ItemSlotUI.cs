using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image selectedBorder;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;

    private RectTransform rectTransform;
    private RectTransform borderRt;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        borderRt = selectedBorder.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (rectTransform.rect.width != borderRt.rect.width)
        {
            Util.MatchRectTransformSize(
                borderRt,
                rectTransform,
                rectTransform.rect.width + 18,
                rectTransform.rect.height
            );
        }
    }

    public float Height => rectTransform.rect.height;

    public void SetSelected(bool selected)
    {
        selectedBorder.gameObject.SetActive(selected);
    }

    public void SetNameAndPrice(UsableObjectSO usableObjectSO)
    {
        nameText.text = usableObjectSO.GetObjectName();
        priceText.text = $"$ {usableObjectSO.GetPrice()}";
    }
}
