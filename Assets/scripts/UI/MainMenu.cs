using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayLocalCoop()
    {
      GameSessions.Instance.Mode = GameSessions.GameMode.Local;
        SceneManager.LoadSceneAsync("LocalCoop"); // scene 1
    }

    public void GoOnlineMenu()
    {
        SceneManager.LoadSceneAsync("OnlineMenu"); // scene 2
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        // Application.Quit(); // enable in build
    }
}
