using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class OnlineMenuUI : MonoBehaviour
{
    public TMP_InputField ipField;
    public GameObject characterPrefabA;
    public GameObject characterPrefabB;
    public bool chooseAByDefault = true;


        private byte selectedIndex;
    // public void SelectCharacterA() { GameSessions.Instance.OnlinePlayerPrefab = characterPrefabA; }
    // public void SelectCharacterB() { GameSessions.Instance.OnlinePlayerPrefab = characterPrefabB; }

    void Start()
    {

        // if (GameSessions.Instance.OnlinePlayerPrefab == null)
        //     GameSessions.Instance.OnlinePlayerPrefab = chooseAByDefault ? characterPrefabA : characterPrefabB;

        // if (ipField) ipField.text = GameSessions.Instance.ServerIp;

        selectedIndex = (byte)(chooseAByDefault ? 0 : 1);
          if (ipField) ipField.text = GameSessions.Instance.ServerIp;
    }

      public void SelectCharacterA() => selectedIndex = 0;
    public void SelectCharacterB() => selectedIndex = 1;

    public void Host()
    {
        GameSessions.Instance.Mode = GameSessions.GameMode.OnlineHost;
         GameSessions.Instance.SelectedCharIndex = selectedIndex;
         SceneManager.LoadSceneAsync("Online_GameScene");// scene 3
    }

    public void Join()
    {
        if (ipField) GameSessions.Instance.ServerIp = ipField.text;

        GameSessions.Instance.Mode = GameSessions.GameMode.OnlineClient;
         GameSessions.Instance.SelectedCharIndex = selectedIndex;
         SceneManager.LoadSceneAsync("Online_GameScene"); // scene 3
    }

    public void BackToMain()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
