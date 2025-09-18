using UnityEngine;
using TMPro;
using Unity.Mathematics;

public class ChoicePedestals : MonoBehaviour
{
      public CraftingRecipeSO recipe;

      //ui
    public Canvas promptCanvas;
    public TMP_Text promptText;
    public GameObject hightlightVFX;
    public RadialHoldUI holdUI;

    private CanvasGroup _group;
    private int _occupantCount = 0;

    private void Awake()
    {
        if (promptCanvas)
        {
            _group = promptCanvas.GetComponent<CanvasGroup>();
            if (!_group) _group = promptCanvas.gameObject.AddComponent<CanvasGroup>();
        }

         if (promptText && recipe)
                promptText.text = $"Make <b>{recipe.name}</b>?\n<alpha=#AA>Confirm = choose\nCancel = back";

        SetPrompt(false);
        if (hightlightVFX) hightlightVFX.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        var id = other.GetComponentInParent<PlayerIdentity>();
        if (!id) return;

        _occupantCount++;

        SetPrompt(true);
        if (holdUI)
        {
            holdUI.Begin(id.playerIndex, OnHoldFinshed);
        }
     

        if (hightlightVFX)
        {
            hightlightVFX.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var id = other.GetComponentInParent<PlayerIdentity>();
        if (!id) return;

        _occupantCount = Mathf.Max(0, _occupantCount - 1);

          if (holdUI)
        {
            holdUI.Cancel(id.playerIndex);
        }

        if (_occupantCount == 0)
        {
            SetPrompt(false);
            if (hightlightVFX)
            {
                hightlightVFX.SetActive(false);
            }
        }
    }

    private void OnHoldFinshed(int playerIndex)
    {
        MatchManger.Instance?.ConfirmRecipeForPlayer(playerIndex, recipe);

        if (_occupantCount == 0) SetPrompt(false);
        if (hightlightVFX) hightlightVFX.SetActive(false);

    }

    private void SetPrompt(bool show)
    {
        if (!promptCanvas) return;

        if (_group)
        {
            _group.alpha = show ? 1f : 0f;
            _group.interactable = show;
            _group.blocksRaycasts = show;
        }
        else
        {
            promptCanvas.enabled = show;
        }


    }
}
