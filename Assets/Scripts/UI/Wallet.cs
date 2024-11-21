using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    public static Wallet Instance { get; private set; }

    public int revenue { get; private set; }

    [SerializeField] private int money;
    [SerializeField] private TextMeshProUGUI revenueText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI floatingTextPrefab;
    [SerializeField] private bool isPractice;

    private void Awake()
    {
        Instance = this;
        revenue = 0;
        UpdateMoneyText();
    }

    private void UpdateMoneyText()
    {
        if (isPractice)
        {
            moneyText.text = "Money: $ ∞";
        }
        else
        {
            // TODO: make this red if negative, green if positive / 0
            moneyText.text = "Money: $ " + money;
        }
    }

    private void UpdateRevenueText()
    {
        revenueText.text = $"Revenue: $ {revenue}";
    }

    public float Money => money;

    public void AddMoney(int checkAmount, int tipAmount)
    {
        int totalAmount = checkAmount + tipAmount;
        if (!isPractice)
        {
            money += totalAmount;
        }
        revenue += totalAmount;

        string displayText = $"+ $ {checkAmount} (Check)";
        if (tipAmount > 0)
        {
            displayText += $"\n+ $ {tipAmount} (Tip)";
        }

        ShowFloatingText(displayText, Color.green);
        UpdateMoneyText();
        UpdateRevenueText();
    }

    public void TakeMoney(int amount, string description = null)
    {
        if (!isPractice)
        {
            money -= amount;
        }
        string displayText = $"- $ {amount}";
        if (description != null)
        {
            displayText += $" ({description})";
        }
        ShowFloatingText(displayText, Color.red);
        UpdateMoneyText();
    }

    private void ShowFloatingText(string text, Color color)
    {

        TextMeshProUGUI floatingText = Instantiate(floatingTextPrefab, moneyText.transform.parent);
        floatingText.text = text;
        floatingText.color = color;

        // floatingText.transform.localPosition = moneyText.transform.localPosition + new Vector3(0, -90, 0);

        StartCoroutine(FadeAndDestroyText(floatingText));
    }

    private IEnumerator FadeAndDestroyText(TextMeshProUGUI floatingText)
    {
        float duration = 2f;
        float elapsed = 0f;
        Color originalColor = floatingText.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            floatingText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - (elapsed / duration));
            yield return null;
        }

        Destroy(floatingText.gameObject);
    }
}
