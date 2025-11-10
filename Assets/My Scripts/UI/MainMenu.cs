using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField ipInputField;
    
    [Header("Config")]
    public string gameplaySceneName = "GameScene";

    public void OnHostClicked()
    {
        var gs = GameSessions.Instance;
        if (!gs)
        {
            Debug.LogError("[MainMenuUI] No GameSessions instance!");
            return;
        }

        gs.Mode = GameSessions.GameMode.OnlineHost;
        gs.SelectedCharIndex = 0;   // Host = Character A

        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnJoinClicked()
    {
        var gs = GameSessions.Instance;
        if (!gs)
        {
            Debug.LogError("[MainMenuUI] No GameSessions instance!");
            return;
        }

        gs.Mode = GameSessions.GameMode.OnlineClient;
        gs.SelectedCharIndex = 1;   // Client = Character B

        if (ipInputField && !string.IsNullOrWhiteSpace(ipInputField.text))
        {
            gs.ServerIp = ipInputField.text;
        }

        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
