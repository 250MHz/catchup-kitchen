using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI revenueText;
    [SerializeField] private TextMeshProUGUI roundText;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void ShowGameOverUI(int revenue, int round)
    {
        gameObject.SetActive(true);
        revenueText.text = $"Revenue: ${revenue}";
        roundText.text = $"Round {round}";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
