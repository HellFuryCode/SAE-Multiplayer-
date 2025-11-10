using UnityEngine;

public class GameSessions : MonoBehaviour
{
    public static GameSessions Instance { get; private set; }

    public enum GameMode { Local, OnlineHost, OnlineClient }

    [Header("Session Mode")]
    public GameMode Mode = GameMode.Local;

    [Header("Online Connection")]
    public string ServerIp = "127.0.0.1";
    [Min(1)] public ushort ServerPort = 7777;

    [Header("Selection")]
    /// <summary>0 = Character A, 1 = Character B (sent as connection payload)</summary>
    public byte SelectedCharIndex = 0;

    // (Legacy/unused in new flow, but keep if other code still references it)
    public GameObject OnlinePlayerPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[GameSessions] Duplicate detected in scene, destroying this one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (SelectedCharIndex > 1) SelectedCharIndex = 1; // clamp to A/B
        if (ServerPort == 0) ServerPort = 7777;
    }
#endif
}
