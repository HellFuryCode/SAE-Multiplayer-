using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerChoice : MonoBehaviour
{
    private PlayerIdentity _id;
    private ChoicePedestals _current;    // the pedestal I'm standing on

    void Awake() { _id = GetComponent<PlayerIdentity>(); }

    public void EnterPedestal(ChoicePedestals p) { _current = p; }
    public void ExitPedestal(ChoicePedestals p)  { if (_current == p) _current = null; }

    public void OnConfirm(InputValue v)
    {
        if (!v.isPressed || _current == null || _current.recipe == null) return;
        MatchManger.Instance?.ConfirmRecipeForPlayer(_id.playerIndex, _current.recipe);
    }

    public void OnCancel(InputValue v)
    {
        if (!v.isPressed) return;
          MatchManger.Instance?.ClearRecipeForPlayer(_id.playerIndex);
    }
}
