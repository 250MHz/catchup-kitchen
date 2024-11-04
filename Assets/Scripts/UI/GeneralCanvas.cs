using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralCanvas : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private RectTransform canvasRt;
    private RectTransform panelRt;

    private void Awake()
    {
        canvasRt = GetComponent<RectTransform>();
        panelRt = panel?.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (canvasRt.rect.width / 2 != panelRt.rect.width
            || canvasRt.rect.height / 2 != panelRt.rect.height)
        {
            Util.SetRectTransformSize(
                panelRt,
                canvasRt.rect.width / 2,
                canvasRt.rect.height / 2
            );
        }
    }
}
