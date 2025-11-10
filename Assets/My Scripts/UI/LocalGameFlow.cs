using System.Collections;
using UnityEngine;

public class LocalGameFlow : MonoBehaviour
{
    public JoinSystem joinSystem;             
        public JoinUI joinUI;                    

 //   public GameObject[] enableOnStart;       
//    public Behaviour[]  disableOnStart;       

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
     
      //  Time.timeScale = 0f;

        if (joinUI)
        {
            var go = joinUI.gameObject;
            if (!go.activeSelf) go.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && joinUI) joinUI.HideNow();     // force hide
        if (Input.GetKeyDown(KeyCode.T)) Time.timeScale = 1f; // force unpause
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
        // // Instantly hide the join panel
        // if (joinUI) joinUI.HideNow(); 

        // foreach (var b in disableOnStart)
        //     if (b) b.enabled = false;

        // // Enable gameplay systems 
        // foreach (var go in enableOnStart)
        //     if (go) go.SetActive(true);


        // Time.timeScale = 1f;
        
         Debug.Log("[LocalGameFlow] StartGameNow()");

   
    if (joinUI != null)
    {
        try
        {
            joinUI.HideNow(); 
            Debug.Log("[LocalGameFlow] joinUI.HideNow() called");
        }
        catch 
        {
            // in case HideNow() missing or throws
            joinUI.gameObject.SetActive(false);
            Debug.LogWarning("[LocalGameFlow] joinUI.HideNow() failed, deactivated GameObject instead");
        }
    }
    else
    {
       
        var go = GameObject.Find("JoinPromptPanel");
        if (go) { go.SetActive(false); Debug.Log("[LocalGameFlow] Fallback: deactivated JoinPromptPanel by name"); }
        else    { Debug.LogWarning("[LocalGameFlow] No joinUI reference and no JoinPromptPanel found"); }
    }

    // // 2) DISABLE JOINING
    // foreach (var b in disableOnStart)
    //     if (b) b.enabled = false;

    // // 3) ENABLE GAMEPLAY ROOTS
    // foreach (var go in enableOnStart)
    //     if (go) go.SetActive(true);

    
    Time.timeScale = 1f;
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;

     //   Debug.Log($"[LocalGameFlow] Unpaused. Time.timeScale={Time.timeScale}. Game started.");
      
        if (joinUI != null)
    {
        var group = joinUI.GetComponent<CanvasGroup>();
        if (group)
        {
            group.interactable = false;
            group.blocksRaycasts = false;
            group.alpha = 0f;
        }
    }

    Debug.Log($"[LocalGameFlow] DONE. Time.timeScale={Time.timeScale}");
    }
    

}
