using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using JetBrains.Annotations;
using Unity.Services.Lobbies.Models;

public class JoinSystem : MonoBehaviour
{
    public enum CharacterChoice { A, B }

    private LocalCoOpBinder binder;
    public GameObject playerPrefab_1_A;
    public GameObject playerPrefab_2_B;

    public Transform[] spawnPoints;

    public Key kb1JoinKey = Key.F;      //joins palyer with wasd
    public Key kb2JoinKey = Key.Enter; //joins player with arrows

    public CharacterChoice nextJoinCharacter = CharacterChoice.A;
    public System.Action<int> OnLocalPlayerJoined;
    public UnityEngine.InputSystem.PlayerInputManager playerInputManager; //asssign in the inscpetor

    public int _NextSpawnIndex = 0;
    private bool kb1Taken = false;
    private bool kb2Taken = false;

    private HashSet<Gamepad> joinedPads = new HashSet<Gamepad>(); //shows which gamepades already joined (not nesscary if using PlayerinoutManger)


    private void Reset()
    {
        if (!playerInputManager)  //auto find
        {
            playerInputManager = FindAnyObjectByType<UnityEngine.InputSystem.PlayerInputManager>();
        }
    }

    private void Start()
    {
        binder = FindFirstObjectByType<LocalCoOpBinder>();
    }


    private void Update()
    {
        //the keyboard joins 
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (!kb1Taken && kb[kb1JoinKey].wasPressedThisFrame)
            {
                SpawnKeyboardPlayer(isKB1: true);
            }

            if (!kb2Taken && kb[kb2JoinKey].wasPressedThisFrame || kb.rightCtrlKey.wasPressedThisFrame)
            {
                SpawnKeyboardPlayer(isKB1: false);
            }

            if (kb != null)
            {
                if (kb.digit1Key.wasPressedThisFrame)
                {
                    ChooseCharacter_1_A();
                }

                if (kb.digit2Key.wasPressedThisFrame)
                {
                    ChooseCharacter_2_B();
                }
            }
        }
    }

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

    private void ApplyNextPrefabToPIM()
    {
        if (!playerInputManager)
        {
            return;
        }

        playerInputManager.playerPrefab = (nextJoinCharacter == CharacterChoice.A) ? playerPrefab_1_A : playerPrefab_2_B;
    }


    private void SpawnKeyboardPlayer(bool isKB1)
    {
        if (binder && binder.Count >= 2)
        {
            return;  //cap at 2
        }

        var prefab = (nextJoinCharacter == CharacterChoice.A) ? playerPrefab_1_A : playerPrefab_2_B;

        var spawn = GetNextSpawn();

        var go = Instantiate(prefab, spawn.position, spawn.rotation);

        var pi = go.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (pi)
        {
            pi.enabled = false;  //keyboard players use manual inputs  wasd or arrows
        }

        var pm = go.GetComponent<PlayerScript_Multi>();
        if (pm)
        {
            pm.keyboardProfile = isKB1
            ? PlayerScript_Multi.KeyboardProfile.WASD
            : PlayerScript_Multi.KeyboardProfile.Arrows;

            if (!pm.Camera && Camera.main) pm.Camera = Camera.main.transform;
            if (binder) binder.Register(pm);
        }

        if (isKB1)
        {
            kb1Taken = true;
        }

        else
        {
            kb2Taken = true;
        }

            int slot = -1;
        if (binder) slot = binder.Register(pm);
        OnLocalPlayerJoined?.Invoke(slot);
    }

    private Transform GetNextSpawn()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return transform;
        }

        var t = spawnPoints[_NextSpawnIndex % spawnPoints.Length];
        _NextSpawnIndex++;
        return t;
    }

    public void OnPlayerJoined(UnityEngine.InputSystem.PlayerInput playerInput)  //controllers aka the better way to play
    {
        var pm = playerInput.GetComponent<PlayerScript_Multi>();

        if (pm && !pm.Camera && Camera.main)
        {
            pm.Camera = Camera.main.transform;
        }

        var pad = playerInput.devices.Count > 0 ? playerInput.devices[0] as Gamepad : null;

        if (pad != null)
        {
            joinedPads.Add(pad);
        }

        if (binder)
        {
            binder.Register(pm);
        }

        int slot = -1;
        if (binder && pm) slot = binder.Register(pm);
        OnLocalPlayerJoined?.Invoke(slot);
    }
}
