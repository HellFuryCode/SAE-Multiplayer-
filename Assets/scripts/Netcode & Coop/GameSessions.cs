using UnityEngine;


public class GameSessions : MonoBehaviour
{
    public static GameSessions Instance { get; private set; }
    public enum GameMode { Local, OnlineHost, OnlineClient }

    public GameMode Mode = GameMode.Local;

    [Header("Online Settings")]
    public string ServerIp = "127.0.0.1";  // for LAN/direct IP
    public ushort ServerPort = 7777; //keyword that represents a 16-bit unsigned intege

     public GameObject OnlinePlayerPrefab; // A or B

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
