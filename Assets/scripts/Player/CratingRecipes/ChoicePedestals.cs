using UnityEngine;
using TMPro;
using Unity.VisualScripting;
public class ChoicePedestals : MonoBehaviour
{
      public CraftingRecipeSO recipe;
    public Canvas promptCanvas;
    public TMP_Text promptText;
    public GameObject hightlightVFX;

    private PlayerIdentity _P1ID, _P2ID;

    private void Awake()
    {
        SetPrompt(false);
        if (hightlightVFX) hightlightVFX.SetActive(false);
    }

    private void OTriggerEnter(Collider other)
    {
        var id = other.GetComponentInParent<PlayerIdentity>();
        if (!id) return;

        if (id.playerIndex == 0) _P1ID = id;
        if (id.playerIndex == 1) _P2ID = id;

        SetPrompt(true);

        if (hightlightVFX)
        {
            hightlightVFX.SetActive(true);
        }
    }

    private void OTriggerExit(Collider other)
    {
        var id = other.GetComponentInParent<PlayerIdentity>();
        if (!id) return;

        if (id.playerIndex == 0) _P1ID = null;
        if (id.playerIndex == 1) _P2ID = null;

        if (_P1ID == null && _P2ID == null)
        {
            SetPrompt(false);
            if (hightlightVFX)
            {
                hightlightVFX.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (!_P1ID && !_P2ID) return;

        bool confirm = UnityEngine.InputSystem.Keyboard.current?.enterKey.wasPressedThisFrame == true
                         || UnityEngine.InputSystem.Gamepad.current?.buttonSouth.wasPressedThisFrame == true;

        bool cancel = UnityEngine.InputSystem.Keyboard.current?.backspaceKey.wasPressedThisFrame == true
                        || UnityEngine.InputSystem.Gamepad.current?.buttonEast.wasPressedThisFrame == true;


        if (confirm)
        {
            if (_P1ID) MatchManger.Instance?.ConfirmRecipeForPlayer(0, recipe);
            if (_P2ID) MatchManger.Instance?.ConfirmRecipeForPlayer(1, recipe);

            SetPrompt(false); //autohide after chosen

            if (hightlightVFX)
            {
                hightlightVFX.SetActive(false);
            }
        }
        // else if (cancel)
        // {
        //     if (_P1ID) MatchManger.Instance?.ClearRecipeForPlayer(0, recipe);
        //     if (_P2ID) MatchManger.Instance?.ClearRecipeForPlayer(1, recipe);
        // }
    }

    private void SetPrompt(bool show)
    {
        if (promptCanvas)
        {
            promptCanvas.enabled = show;
        }

        if (promptText && recipe)
        {
            promptText.text = $"Make **{recipe.name}**?\n<alpha=#AA>Enter / A = choose\nBackSpace / O = back";
        }
    }
}
