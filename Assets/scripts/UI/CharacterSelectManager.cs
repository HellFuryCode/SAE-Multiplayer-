using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    public GameObject[] characterPrefabs;
    public bool autoStartWhenAllReady = true;

    private int readyCount = 0;

    private void Start()
    {
        var persistence = CharacterSelectPersistence.Instance;
        if (persistence == null)
        {
            var go = new GameObject("CharacterSelectPersistence");
            persistence = go.AddComponent<CharacterSelectPersistence>();
        }

        persistence.characterPrefabs.Clear();
        if (characterPrefabs != null)
        {
            persistence.characterPrefabs.AddRange(characterPrefabs);
        }

        persistence.Clear();    // Clear any selections from other runs.
    }

    public void OnPlayerReady()
    {
        readyCount++;

        if (!autoStartWhenAllReady) return;

        var manager = FindAnyObjectByType<UnityEngine.InputSystem.PlayerInputManager>();

        if (manager != null && readyCount >= manager.playerCount && manager.playerCount > 0)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }
}
