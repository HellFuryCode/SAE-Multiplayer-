using UnityEngine;
using TMPro;

public class WinOrLoseUI : MonoBehaviour
{
    public CanvasGroup group;
    public TMP_Text titleText;
    public TMP_Text detailsText;

    private void Awake()
    {
        if (!group) group = GetComponent<CanvasGroup>();
        Hide(true);
    }

    public void ShowResults(int p1Score, int p2Score)
    {
        string title;
        if (p1Score > p2Score)
        {
            title = "Player 1 Wins!";
        }
        else if (p1Score > p2Score)
        {
            title = "Player 2 Wins!";
        }
        else
        {
            title = "Tie!";
        }

        if (titleText) titleText.text = title;
        if (detailsText) detailsText.text = $"P1: {p1Score}\nP2: {p2Score}";

        Show();

    }

    public void Show()
    {
        gameObject.SetActive(true);
        group.alpha = 1f;
        group.blocksRaycasts = true;
        group.interactable = true;
        Time.timeScale = 0f;
    }

    public void Hide(bool instant = false)
    {
        if (instant)
        {
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;
            Time.timeScale = 1f;
        }
    }

}


