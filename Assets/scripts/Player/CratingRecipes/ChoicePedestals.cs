using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Unity.Mathematics;
public class ChoicePedestals : MonoBehaviour
{
      public CraftingRecipeSO recipe;
    public Canvas promptCanvas;
    public TMP_Text promptText;
    public GameObject hightlightVFX;

    private int _occupantCount = 0;

    private void Awake()
    {
        SetPrompt(false);
        if (hightlightVFX) hightlightVFX.SetActive(false);

         if (promptText && recipe)
            promptText.text = $"Make <b>{recipe.name}</b>?\n<alpha=#AA>Confirm = choose\nCancel = back";
    }

    private void OnTriggerEnter(Collider other)
    {
        var chooser = other.GetComponentInParent<PlayerChoice>();
        if (!chooser) return;

        _occupantCount++;
        chooser.EnterPedestal(this);
        
        SetPrompt(true);

        if (hightlightVFX)
        {
            hightlightVFX.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var chooser = other.GetComponentInParent<PlayerChoice>();
        if (!chooser) return;

        _occupantCount = Mathf.Max(0, _occupantCount - 1);
        chooser.ExitPedestal(this);

        if (_occupantCount == 0)
        {
            SetPrompt(false);
            if (hightlightVFX)
            {
                hightlightVFX.SetActive(false);
            }
        }
    }

    private void SetPrompt(bool show)
    {
        if (promptCanvas)
        {
            promptCanvas.gameObject.SetActive(show);
        }

      
    }
}
