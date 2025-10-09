using System.Collections;
using UnityEngine;

public class LocalGameFlow : MonoBehaviour
{
    public JoinSystem joinSystem;             
        public JoinUI joinUI;                    

    public GameObject[] enableOnStart;       
    public Behaviour[]  disableOnStart;       

    public bool waitForBothPlayers = false;   

    private int  _joinedCount = 0;
    private bool _starting = false;

    private void Reset()
    {
       
        if (!joinSystem) joinSystem = FindFirstObjectByType<JoinSystem>();      
        if (!joinUI)     joinUI     = FindFirstObjectByType<JoinUI>();          
       
    }

    private void Awake()
    {
     
        Time.timeScale = 0f;

        if (joinUI)
        {
            var go = joinUI.gameObject;
            if (!go.activeSelf) go.SetActive(true);
        }
    }

    private void OnEnable()
    {
        if (!joinSystem) joinSystem = FindFirstObjectByType<JoinSystem>(); 
        if (joinSystem != null)
        {
            joinSystem.OnLocalPlayerJoined += HandlePlayerJoined;          
        }
    }

    private void OnDisable()
    {
        if (joinSystem != null)
            joinSystem.OnLocalPlayerJoined -= HandlePlayerJoined;          
    }

    private void HandlePlayerJoined(int slotIndex)
    {
    
        _joinedCount = Mathf.Max(_joinedCount, slotIndex + 1);

        if (_starting) return;

        bool conditionMet = waitForBothPlayers ? (_joinedCount >= 2) : (_joinedCount >= 1);
        if (conditionMet)
        {
            _starting = true;

            
            StartGameNow();
        }
    }
    private void StartGameNow()
    {
        // Instantly hide the join panel
        if (joinUI) joinUI.HideNow(); 
   
        foreach (var b in disableOnStart)
            if (b) b.enabled = false;

        // Enable gameplay systems 
        foreach (var go in enableOnStart)
            if (go) go.SetActive(true);

   
        Time.timeScale = 1f;
    }


}
