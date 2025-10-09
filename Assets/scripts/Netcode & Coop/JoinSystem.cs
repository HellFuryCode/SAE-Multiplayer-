using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class JoinSystem : MonoBehaviour
{
    public enum CharacterChoice { A, B }

    [Header("Prefabs (LOCAL, non-networked)")]
    public GameObject playerPrefab_1_A;
    public GameObject playerPrefab_2_B;

    [Header("Spawning")]
    public Transform[] spawnPoints;

    [Header("Keyboard Join Keys")]
    public Key kb1JoinKey = Key.F;        // P1 (WASD)
    public Key kb2JoinKey = Key.Enter;    // P2 (Arrows) – also handles Return/NumpadEnter/RightCtrl/RightShift

    [Header("Controller Join (via PlayerInputManager)")]
    public PlayerInputManager playerInputManager; // assign in inspector (optional; auto-found in Reset)

    [Header("Character choice for NEXT join")]
    public CharacterChoice nextJoinCharacter = CharacterChoice.A;

    // Event: slotIndex 0 => Player1, 1 => Player2
    public System.Action<int> OnLocalPlayerJoined;

    // --- internals ---
    private LocalCoOpBinder binder;
    private int _nextSpawnIndex = 0;
    private bool kb1Taken = false;
    private bool kb2Taken = false;
    private readonly HashSet<Gamepad> joinedPads = new HashSet<Gamepad>();

    private void Reset()
    {
        if (!playerInputManager)
            playerInputManager = FindFirstObjectByType<PlayerInputManager>();
    }

    private void Start()
    {
        binder = FindFirstObjectByType<LocalCoOpBinder>();
        ApplyNextPrefabToPIM(); // make sure controllers use the current selection
    }

    private void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // P1 (WASD) – single key join (default F)
        if (!kb1Taken && kb[kb1JoinKey].wasPressedThisFrame)
        {
            SpawnKeyboardPlayer(isKB1: true);
            Debug.Log("[Join] P1 keyboard join key pressed");
        }

        // P2 (Arrows) – handle common variants + inspector key + RightCtrl/RightShift
        bool p2JoinPressed =
            kb.enterKey.wasPressedThisFrame ||
            kb.numpadEnterKey.wasPressedThisFrame ||
            kb.rightCtrlKey.wasPressedThisFrame ||
            kb.rightShiftKey.wasPressedThisFrame ||
            kb[kb2JoinKey].wasPressedThisFrame;

        if (!kb2Taken && p2JoinPressed)
        {
            SpawnKeyboardPlayer(isKB1: false);
            Debug.Log("[Join] P2 keyboard join key pressed");
        }

        // Optional hotkeys to flip next character (comment out if you don't want it)
        if (kb.digit1Key.wasPressedThisFrame) ChooseCharacter_1_A();
        if (kb.digit2Key.wasPressedThisFrame) ChooseCharacter_2_B();
    }

    // ----- Character selection -----
    public void ChooseCharacter_1_A()
    {
        nextJoinCharacter = CharacterChoice.A;
        ApplyNextPrefabToPIM();
    }

    public void ChooseCharacter_2_B()
    {
        nextJoinCharacter = CharacterChoice.B;
        ApplyNextPrefabToPIM();
    }

    private void FlipNext()
    {
        nextJoinCharacter = (nextJoinCharacter == CharacterChoice.A) ? CharacterChoice.B : CharacterChoice.A;
        ApplyNextPrefabToPIM();
    }

    private void ApplyNextPrefabToPIM()
    {
        if (!playerInputManager) return;

        playerInputManager.playerPrefab =
            (nextJoinCharacter == CharacterChoice.A) ? playerPrefab_1_A : playerPrefab_2_B;
    }

    // ----- Spawning -----
    private void SpawnKeyboardPlayer(bool isKB1)
    {
        if (binder && binder.Count >= 2) return; // cap at 2 players

        var prefab = (nextJoinCharacter == CharacterChoice.A) ? playerPrefab_1_A : playerPrefab_2_B;
        var spawn = GetNextSpawn();

        var go = Instantiate(prefab, spawn.position, spawn.rotation);

        // Disable PlayerInput for keyboard splits (manual readers in PlayerScript_Multi)
        var pi = go.GetComponent<PlayerInput>();
        if (pi) pi.enabled = false;

        var pm = go.GetComponent<PlayerScript_Multi>();
        if (pm)
        {
            pm.keyboardProfile = isKB1 ? PlayerScript_Multi.KeyboardProfile.WASD
                                       : PlayerScript_Multi.KeyboardProfile.Arrows;

            if (!pm.Camera && Camera.main) pm.Camera = Camera.main.transform;
        }

        int slot = (binder && pm) ? binder.Register(pm) : -1;
        OnLocalPlayerJoined?.Invoke(slot);

        if (isKB1) kb1Taken = true; else kb2Taken = true;

        // Auto-alternate next join to the other character
        FlipNext();
    }

    private Transform GetNextSpawn()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return transform;
        var t = spawnPoints[_nextSpawnIndex % spawnPoints.Length];
        _nextSpawnIndex++;
        return t;
    }

    // ----- PlayerInputManager callback (controllers) -----
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (binder && binder.Count >= 2)
        {
            // Optional: kick/disable extra joins if you want a hard cap
            Debug.LogWarning("[Join] Third controller tried to join, ignoring.");
            return;
        }

        var pm = playerInput.GetComponent<PlayerScript_Multi>();
        if (pm && !pm.Camera && Camera.main)
            pm.Camera = Camera.main.transform;

        if (playerInput.devices.Count > 0)
        {
            var pad = playerInput.devices[0] as Gamepad;
            if (pad != null) joinedPads.Add(pad);
        }

        int slot = (binder && pm) ? binder.Register(pm) : -1;
        OnLocalPlayerJoined?.Invoke(slot);

        // Alternate the next prefab for the next controller join
        FlipNext();
    }
}
