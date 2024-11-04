using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    public static Wallet Instance { get; private set; }

    [SerializeField] private int money;
    [SerializeField] private TextMeshProUGUI moneyText;

    private void Awake()
    {
        Instance = this;
        UpdateMoneyText();
    }

    private void UpdateMoneyText()
    {
        // TODO: make this red if negative, green if positive / 0
        moneyText.text = "Money\n$ " + money;
    }

    public float Money => money;

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyText();
    }

    public void TakeMoney(int amount)
    {
        money -= amount;
        UpdateMoneyText();
    }
}
