using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountSelectorUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI priceText;

    private int minCount = 1;
    private int maxCount = 99;
    private int currentCount;
    private int pricePerUnit;

    public void ShowSelector(int pricePerUnit, int currentCount = 1)
    {
        this.pricePerUnit = pricePerUnit;
        this.currentCount = currentCount;
        gameObject.SetActive(true);
        SetValues();
    }

    // Use GetCurrentCount() and Disable() in ShopController when we want
    // to confirm a count

    public int GetCurrentCount()
    {
        return currentCount;
    }

    public int GetTotalPrice()
    {
        return currentCount * pricePerUnit;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnMove(Vector2 moveAmount)
    {
        if (moveAmount.y > 0)
        {
            // Up
            currentCount++;
            if (currentCount > maxCount)
            {
                currentCount = minCount;
            }
        }
        else if (moveAmount.x > 0)
        {
            // Right
            currentCount += 10;
            if (currentCount > maxCount)
            {
                currentCount -= maxCount;
            }
        }
        else if (moveAmount.y < 0)
        {
            // Down
            currentCount--;
            if (currentCount < minCount)
            {
                currentCount = maxCount;
            }
        }
        else if (moveAmount.x < 0)
        {
            // Left
            currentCount -= 10;
            if (currentCount < minCount)
            {
                currentCount += maxCount;
            }
        }

        SetValues();
    }

    private void SetValues()
    {
        countText.text = "x" + currentCount.ToString("D2");
        priceText.text = $"$ {pricePerUnit * currentCount}";
    }
}
