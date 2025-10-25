using System.Collections;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;


public class MatchManger : MonoBehaviour
{
    public static MatchManger Instance { get; private set; }
    public GameObject pedistalGroup;
    public CamFollow2Players camRig;
    public GameObject startWall;
    public CraftingBowl player1Bowl;
    public CraftingBowl player2Bowl;

    public TMP_Text p1RecipeUI;
    public TMP_Text p2RecipeUI;
    public TMP_Text countDownUI;
    public TMP_Text TimerUI;

    public WinOrLoseUI resultsUI;
    public float roundSeconds = 120f;

    private CraftingRecipeSO _p1Chosen, _p2Chosen;
    private float _timeLeft;
    private bool _roundRunning;

    public CountdownUI countdown; //cavas group needed

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject); return;
        }

         Instance = this;  //fucker stay here
    }

    private void Start()
    {
       // if (camRig) camRig.inputLocked = true;
        if (startWall) startWall.SetActive(true);

        SetRecipeUI(0, null);
        SetRecipeUI(1, null);

        if (countDownUI)
        {
            countDownUI.enabled = false;
        }

        if (GameDirector.Instance)
        {
            GameDirector.Instance.ResetScores();
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

        if (_roundRunning)
        {
            _timeLeft -= Time.deltaTime;
            if (_timeLeft < 0f) _timeLeft = 0f;
            UpdateTimerUI();
            if (_timeLeft <= 0f)
            {
                _roundRunning = false;
                EndRound();
            }
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
            Debug.Log($"[Match] P{playerIndex+1} chose {recipe?.name}");
        if (playerIndex == 0)
        {
            _p1Chosen = recipe; SetRecipeUI(0, recipe);
            if (player1Bowl)
            {
                player1Bowl.SetRecipe(recipe);
            }
        }
        else if (playerIndex == 1)
        {
            _p2Chosen = recipe; SetRecipeUI(1, recipe);
            if (player2Bowl)
            {
                player2Bowl.SetRecipe(recipe);
            }
        }

        if (!_roundRunning && _p1Chosen && _p2Chosen)
        {
              Debug.Log("[Match] Both chosen → starting countdown…");
            StartCoroutine(BeginCountdownThenStart());
        }
    }
    
     public void ClearRecipeForPlayer(int playerIndex)
    {
        if (playerIndex == 0)
        {
            _p1Chosen = null; SetRecipeUI(0, null);
        }
        else if (playerIndex == 1)
        {
            _p2Chosen = null; SetRecipeUI(1, null);
        }
    }

    private void EndRound()
    {
        int p1 = GameDirector.Instance ? GameDirector.Instance.GetScore(0) : 0;
        int p2 = GameDirector.Instance ? GameDirector.Instance.GetScore(1) : 0;

        if (resultsUI) resultsUI.ShowResults(p1, p2);
    }
 private IEnumerator BeginCountdownThenStart()
    {
        var seq = new[]
        {
            "3" , "2", "1", "<b>GO!</b>"//bitbh
        };

        if (countdown) yield return StartCoroutine(countdown.PlayCountDown(seq)); //bitch 

        if (pedistalGroup) pedistalGroup.SetActive(false);
        if (startWall) startWall.SetActive(false);
        //  if (camRig) camRig.inputLocked = false;

        _timeLeft = roundSeconds;
        _roundRunning = true;
        UpdateTimerUI();
    }
}
