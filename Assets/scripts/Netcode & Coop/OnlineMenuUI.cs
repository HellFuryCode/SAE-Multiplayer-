using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class OnlineMenuUI : MonoBehaviour
{
    public TMP_InputField ipField;             
    public GameObject characterPrefabA;       
    public GameObject characterPrefabB;      
    public bool chooseAByDefault = true;

    public void SelectCharacterA() { GameSessions.Instance.OnlinePlayerPrefab = characterPrefabA; }
    public void SelectCharacterB() { GameSessions.Instance.OnlinePlayerPrefab = characterPrefabB; }

    void Start()
    {
     
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
