using System.Collections;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;


public class MatchManger : MonoBehaviour
{
    public static MatchManger Instance { get; private set; }

    public CamFollow2Players camRig;
    public GameObject startWall;
    public CraftingBowl player1Bowl;
    public CraftingBowl player2Bowl;

    public TMP_Text p1RecipeUI;
    public TMP_Text p2RecipeUI;
    public TMP_Text countDownUI;
    public TMP_Text TimerUI;

    public float roundSeconds = 120f;

    private CraftingRecipeSO _p1Chosen, _p2Chosen;
    private float _timeLeft;
    private bool _roundRunning;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject); return;
            // Instance = this;
        }
    }

    private void Start()
    {
        if (camRig) camRig.inputLocked = true;
        if (startWall) startWall.SetActive(true);

        SetRecipeUI(0, null);
        SetRecipeUI(1, null);

        if (countDownUI)
        {
            countDownUI.enabled = false;
        }

        _roundRunning = false;
        _timeLeft = roundSeconds;
        UpdateTimerUI();
    }

    private void Update()
    {
        if (!_roundRunning) return;

        _timeLeft -= Time.deltaTime;
        if (_timeLeft < 0f) _timeLeft = 0f;
        UpdateTimerUI();

        if (_timeLeft <= 0f)
        {
            _roundRunning = false;
           // EndRound();
        }
    }

    private void UpdateTimerUI()
    {
        if (!TimerUI) return;
        int t = Mathf.CeilToInt(_timeLeft);
        int m = t / 60;
        int s = t % 60;
        TimerUI.text = $"{m}:{s:00}";
    }

    private void SetRecipeUI(int playerIndex, CraftingRecipeSO so)
    {
        var txt = (playerIndex == 0) ? p1RecipeUI : p2RecipeUI;
        if (!txt) return;
        txt.text = so ? $"Chosen: {so.name}" : "<alpha=#66>No recipe chosen";
    }

    public void ConfirmRecipeForPlayer(int playerIndex, CraftingRecipeSO recipe)
    {
        if (playerIndex == 0)
        {
            _p1Chosen = recipe; SetRecipeUI(0, recipe);
        }
        else if (playerIndex == 1)
        {
            _p2Chosen = recipe; SetRecipeUI(1, recipe);
        }

        if (playerIndex == 0 && player1Bowl)
        {
            player1Bowl.SetRecipe(recipe);
        }
        if (playerIndex == 1 && player2Bowl)
        {
            player2Bowl.SetRecipe(recipe);
        }

        if (!_roundRunning && _p1Chosen && _p2Chosen)
        {
            // StartCoroutine(BeginCountdownThenStart());
        }
    }

    // public void ClearRecipeForPlayer(int playerIndex)
    // {
    //     if (playerIndex == 0)
    //     {
    //         _p1Chosen = null;
    //         SetRecipeUI(0, null);
    //     }
    //     else
    //     {
    //         _p2Chosen = null;
    //         SetRecipeUI(1, null);
    //     }

    //     IEnumerator BeginCountdownThenStart()
    //     {
    //         if (countDownUI)
    //         {
    //             countDownUI.OnFinished = () =>
    //             {
    //                 if (startWall) startWall.SetActive(false);
    //                 if (camRig) camRig.inputLocked = false;

    //                 _timeLeft = roundSeconds;
    //                 _roundRunning = true;
    //                 UpdateTimerUI();
    //             };
    //         }

    //         countDownUI.Play();
    //     }

    //     yield break;
    // }
}
