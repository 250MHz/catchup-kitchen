using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image barImage;
    private Color safeColor = new Color(106f/255f, 168f/255f, 79f/255f);
    private Color dangerColor = new Color(204f/255f, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        barImage.fillAmount = 0f;
        SetSafeColor();
        Hide();
    }

    public void SetBarFillAmount(float progress)
    {
        barImage.fillAmount = progress;
        if (progress == 0f || progress == 1f)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void SetSafeColor()
    {
        if (barImage.color != safeColor)
        {
            barImage.color = safeColor;
        }
    }

    public void SetDangerColor()
    {
        if (barImage.color != dangerColor)
        {
            barImage.color = dangerColor;
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
