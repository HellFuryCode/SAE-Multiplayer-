using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;

public class CharacterSelctUI : MonoBehaviour
{
    public TMP_Text nameLabel;
    public GameObject readyBadge;
    public GameObject[] characters;
    public int  currentIndex = 0; 
    public float naigationCooldownSeconds = 0.15f;

    private float navigationTimer = 0.0f;
    private bool isReady = false;

    private CharacterSelectPersistence persistence;
    private CharacterSelectManager manager;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        persistence = CharacterSelectPersistence.Instance;
        manager = FindAnyObjectByType<CharacterSelectManager>();
    }

    private void Start()
    {
        if (readyBadge != null) readyBadge.SetActive(false);
        UpdateLabel();
    }

    private void Update()
    {
        if (navigationTimer > 0.0f)
        {
            navigationTimer -= Time.unscaledDeltaTime;
        }
    }

    private void UpdateLabel()
    {
        if (persistence == null || persistence.characterPrefabs == null || persistence.characterPrefabs.Count == 0)
        {
            if (nameLabel != null) nameLabel.text = "(No Characters Confriggured)";
            return;
        }

        if (currentIndex < 0) currentIndex = 0;
        if (currentIndex >= persistence.characterPrefabs.Count) currentIndex = persistence.characterPrefabs.Count - 1;
    }

    
}



//https://www.youtube.com/watch?v=7glCsF9fv3s&list=PLzDRvYVwl53sSmEcIgZyDzrc0Smpq_9fN
//https://www.youtube.com/watch?v=3qlRgICRoeA