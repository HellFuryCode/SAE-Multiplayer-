using UnityEngine;
using TMPro;
using System;


public class GameDirector : MonoBehaviour
{
    public static GameDirector Instance { get; private set; }  //remeber its ; not :

    public TMP_Text p1ScoreText;
    public TMP_Text p2ScoreText;

    int[] scores = new int[4];

    public event Action<int, int> OnScoreChanged;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;  //this was the resaon this fucker wasnt working
    }

    public void ResetScores()
    {
        for (int i = 0; i < scores.Length; i++) scores[i] = 0;
        {
            RefreshUI();
        }
    }

    public int GetScore(int playerIndex)
    {
        playerIndex = Mathf.Clamp(playerIndex, 0, scores.Length - 1);
        return scores[playerIndex];
    }

    public void AddScore(int playerIndex, int points)
    {
        playerIndex = Mathf.Clamp(playerIndex, 0, scores.Length - 1);
        scores[playerIndex] += Mathf.Max(1, points);

        RefreshUI();
        OnScoreChanged?.Invoke(playerIndex, scores[playerIndex]);
    }


    private void RefreshUI()
    {
        if (p1ScoreText) p1ScoreText.text = scores[0].ToString();
        if (p2ScoreText) p2ScoreText.text = scores[1].ToString();

    }
}
