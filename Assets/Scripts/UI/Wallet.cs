using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    public static Wallet Instance { get; private set; }

    [SerializeField] private int money;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI floatingTextPrefab;

    [SerializeField] private int deductionAmount;
    [SerializeField] private float roundInterval;
    private bool isGameOver = false;

    private void Awake()
    {
        Instance = this;
        UpdateMoneyText();
    }

    private void Start()
    {
        StartCoroutine(RoundDeductionRoutine());
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
        ShowFloatingText("+ $ " + amount, Color.green);
        UpdateMoneyText();
    }

    public void TakeMoney(int amount)
    {

        money -= amount;
        ShowFloatingText("- $ " + amount, Color.red);
        UpdateMoneyText();
        CheckGameOver();
    }

    private IEnumerator RoundDeductionRoutine()
    {
        while (!isGameOver)
        {
            yield return new WaitForSeconds(roundInterval);
            TakeMoney(deductionAmount);
            Debug.Log($"Deducted {deductionAmount}. Current money: {money}");
        }
    }

    private void CheckGameOver()
    {
        if (Money < 0 && !isGameOver)
        {
            isGameOver = true;
            GameOver();
        }
    }

    private void GameOver()
    {
        
        Debug.Log("Game Over! You have run out of money.");
        
        Time.timeScale = 0f;
    }

    private void ShowFloatingText(string text, Color color)
    {
        
        TextMeshProUGUI floatingText = Instantiate(floatingTextPrefab, moneyText.transform.parent);
        floatingText.text = text;
        floatingText.color = color;

        
        floatingText.transform.localPosition = moneyText.transform.localPosition + new Vector3(0, -60, 0);

        
        StartCoroutine(FadeAndDestroyText(floatingText));
    }

    private IEnumerator FadeAndDestroyText(TextMeshProUGUI floatingText)
    {
        float duration = 1f;
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
