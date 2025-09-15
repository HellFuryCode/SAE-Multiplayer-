using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CharacterSelectPersistence : MonoBehaviour
{
    public class selectedPlayer
    {
        public int characterIndex;
        public string controlScheme;
        public InputDevice device;
    }

    public static CharacterSelectPersistence Instance { get; private set; }

    public List<GameObject> characterPrefabs = new List<GameObject>();

    public List<selectedPlayer> selctions = new List<selectedPlayer>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Clear()
    {
        selctions.Clear();
    }
}
