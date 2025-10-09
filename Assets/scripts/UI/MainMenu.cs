using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayLocalCoop()
    {
    //   GameSessions.Instance.Mode = GameSessions.GameMode.Local;
        SceneManager.LoadSceneAsync("Local_GameScene");
    }

    public void GoOnlineMenu()
    {
        SceneManager.LoadSceneAsync("Online_GameScene"); // scene 2
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        // Application.Quit(); // enable in build
    }
}
