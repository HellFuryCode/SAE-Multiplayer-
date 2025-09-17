using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class GameDirector : MonoBehaviour
{
    public static GameDirector Instance { get; private set; }  //remeber its ; not :

    public TMP_Text p1ScoreText;
    public TMP_Text p2ScoreText;

    int[] scores = new int[4];

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void AddScore(int playerIndex, int points)
    {
        playerIndex = Mathf.Clamp(playerIndex, 0, 3);
        scores[playerIndex] += Mathf.Max(1, points);

        if (p1ScoreText)
        {
            p1ScoreText.text = scores[0].ToString();
        }

        if (p2ScoreText)
        {
            p2ScoreText.text = scores[0].ToString();

        }
        
        //add timer for extra points
    }

}
