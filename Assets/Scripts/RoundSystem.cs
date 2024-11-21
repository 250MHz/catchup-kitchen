using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundSystem : MonoBehaviour
{
    public static RoundSystem Instance { get; private set; }
    public int roundNumber { get; private set; }
    public bool isGameActive { get; private set; }

    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI rentMessageText;
    [SerializeField] private PauseUI pauseUI;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private bool isPractice;

    [Header("Round system logic")]
    [SerializeField, Tooltip("Amount to be paid after round 3 ends")]
    private int initialRentPayment;
    [SerializeField, Tooltip("Amount that rent should increase each time it's paid from rounds 3 to 21, inclusive")]
    private int stageOneRentIncrease;
    [SerializeField, Tooltip("Duration of rounds (seconds) from rounds 1 to 21, inclusive")]
    private float stageOneDuration;

    [Space(10)]
    [SerializeField, Tooltip("Amount that rent should increase each time it's paid from rounds 22 to 29, inclusive")]
    private int stageTwoRentIncrease;
    [SerializeField, Tooltip("Duration of rounds (seconds) from rounds 22 to 29, inclusive")]
    private float stageTwoDuration;

    [Space(10)]
    [SerializeField, Tooltip("Amount that rent should increase each time it's paid from round 30 and onwards")]
    private int stageThreeRentIncrease;
    [SerializeField, Tooltip("Duration of rounds (seconds) from round 30 and onwards")]
    private float stageThreeDuration;

    private float roundTimer;
    private int rentPayment;

    private void Awake()
    {
        Instance = this;
        pauseUI.gameObject.SetActive(false);
        isGameActive = true;
        // UpdateRound() increases rentPayment before it's taking money from
        // Wallet, so we need to subtract earlier
        rentPayment = initialRentPayment - stageOneRentIncrease;
        roundNumber = 0;
        UpdateRentMessageText(initialRentPayment, 6);
    }

    private void Update()
    {
        if (!isGameActive)
        {
            return;
        }
        roundTimer -= Time.deltaTime;
        if (roundTimer <= 0f)
        {
            UpdateRound();
        }
        timerText.text = $"Time: {roundTimer.ToString("0")}";
    }

    private void UpdateRound()
    {
        if (roundNumber >= 0 && roundNumber <= 21)
        {
            roundTimer = stageOneDuration;
            if (roundNumber >= 6 && roundNumber % 3 == 0)
            {
                // 6, 9, 12, 15, 18, 21
                rentPayment += stageOneRentIncrease;
                Wallet.Instance.TakeMoney(rentPayment, "Rent");
                CheckGameOver();
                int nextPayment = rentPayment + (roundNumber + 3 <= 21
                    ? stageOneRentIncrease
                    : stageTwoRentIncrease);
                int nextRound = roundNumber + 3 <= 21
                    ? roundNumber + 3
                    : roundNumber + 2;
                UpdateRentMessageText(nextPayment, nextRound);
            }
        }
        else if (roundNumber >= 22 && roundNumber <= 29)
        {
            roundTimer = stageTwoDuration;
            if (roundNumber % 2 == 1)
            {
                // 23, 25, 27, 29
                rentPayment += stageTwoRentIncrease;
                Wallet.Instance.TakeMoney(rentPayment, "Rent");
                CheckGameOver();
                int nextPayment = rentPayment + (roundNumber + 2 <= 29
                    ? stageTwoRentIncrease
                    : stageThreeRentIncrease);
                int nextRound = roundNumber + 2 <= 29
                    ? roundNumber + 2
                    : roundNumber + 1;
                UpdateRentMessageText(nextPayment, nextRound);
            }
        }
        else if (roundNumber >= 30)
        {
            roundTimer = stageThreeDuration;
            rentPayment += stageThreeRentIncrease;
            Wallet.Instance.TakeMoney(rentPayment, "Rent");
            CheckGameOver();
            UpdateRentMessageText(
                rentPayment + stageThreeRentIncrease, roundNumber + 1
            );
        }
        roundNumber++;
        roundText.text = $"Round: {roundNumber}";
    }

    private void CheckGameOver()
    {
        if (isPractice)
        {
            return;
        }
        if (Wallet.Instance.Money < 0)
        {
            gameOverUI.gameObject.SetActive(true);
            gameOverUI.ShowGameOverUI(Wallet.Instance.revenue, roundNumber);
            isGameActive = false;
        }
    }

    public void OpenPauseUI()
    {
        pauseUI.gameObject.SetActive(true);
        pauseUI.Pause();
    }

    private void UpdateRentMessageText(int nextPayment, int nextRound)
    {
        rentMessageText.text = $"${nextPayment} due after round {nextRound}";
    }
}
