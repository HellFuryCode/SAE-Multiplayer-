using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // if using TextMeshPro

public class OnlineMenuUI : MonoBehaviour
{
    public TMP_InputField ipField;             // assign in Inspector
    public GameObject characterPrefabA;        // assign your A prefab
    public GameObject characterPrefabB;        // assign your B prefab
    public bool chooseAByDefault = true;

    public void SelectCharacterA() { GameSessions.Instance.OnlinePlayerPrefab = characterPrefabA; }
    public void SelectCharacterB() { GameSessions.Instance.OnlinePlayerPrefab = characterPrefabB; }

    void Start()
    {
        // sensible defaults
        if (GameSessions.Instance.OnlinePlayerPrefab == null)
            GameSessions.Instance.OnlinePlayerPrefab = chooseAByDefault ? characterPrefabA : characterPrefabB;

        if (ipField) ipField.text = GameSessions.Instance.ServerIp;
    }

    public void Host()
    {
        GameSessions.Instance.Mode = GameSessions.GameMode.OnlineHost;
        SceneManager.LoadSceneAsync("GameScene"); // scene 3
    }

    public void Join()
    {
        if (ipField) GameSessions.Instance.ServerIp = ipField.text;
        GameSessions.Instance.Mode = GameSessions.GameMode.OnlineClient;
        SceneManager.LoadSceneAsync("GameScene"); // scene 3
    }

    public void BackToMain()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
