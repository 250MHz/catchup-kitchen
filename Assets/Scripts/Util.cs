using UnityEngine;

public static class Util
{

    // Modified from https://discussions.unity.com/t/modify-the-width-and-height-of-recttransform/551868/9
    public static void MatchRectTransformSize(
        RectTransform rt,
        RectTransform other,
        float newWidth,
        float newHeight
    )
    {
        Vector2 myPrevPivot = rt.pivot;
        rt.pivot = other.pivot;
        rt.position = other.position;
        rt.localScale = other.localScale;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        rt.pivot = myPrevPivot;
    }

    public static void SetRectTransformSize(RectTransform rt, float newWidth, float newHeight)
    {
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
    }
}