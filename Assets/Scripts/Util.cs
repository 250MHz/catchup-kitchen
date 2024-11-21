using UnityEngine;
using UnityEngine.InputSystem;

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

    // https://www.reddit.com/r/Unity3D/comments/18zdq3k/comment/l6ykshz/
    public static string GetBindingDisplayStringOrCompositeName(string scheme, InputAction action)
    {
        int count = action.bindings.Count;
        for (int i = 0; i < count; i++)
        {
            InputBinding binding = action.bindings[i];
            bool isBindingMatchingScheme = InputBinding.MaskByGroup(scheme).Matches(binding);
            if (isBindingMatchingScheme && !binding.isComposite && !binding.isPartOfComposite)
            {
                return action.GetBindingDisplayString(i);
            }
            bool isNextBindingWithinRange = i + 1 < count;
            if (isNextBindingWithinRange && binding.isComposite)
            {
                InputBinding nextBinding = action.bindings[i + 1];
                bool isNextBindingMatchingScheme = InputBinding.MaskByGroup(scheme).Matches(nextBinding);
                if (isNextBindingMatchingScheme)
                {
                    // return binding.name;
                    // alternatively use this for built-in composite string constructor:
                    return action.GetBindingDisplayString(i);
                }
            }
        }
        return string.Empty;
    }
}