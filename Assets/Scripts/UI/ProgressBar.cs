using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image barImage;

    // Start is called before the first frame update
    void Start()
    {
        barImage.fillAmount = 0f;
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

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
