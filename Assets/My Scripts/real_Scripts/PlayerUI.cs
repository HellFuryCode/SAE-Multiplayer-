using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Text scoreText;

    private Player player;

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
    }

    private void Update()
    {
        scoreText.text = "Score: " + player.playerScore;
    }
}