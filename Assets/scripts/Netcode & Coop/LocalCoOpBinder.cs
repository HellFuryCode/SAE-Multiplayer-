using UnityEngine;
using System.Collections.Generic;

public class LocalCoOpBinder : MonoBehaviour
{
    public CamFollow2Players cam;
    private readonly List<PlayerScript_Multi> players = new();

    private void Awake()
    {
        if (!cam)
        {
            cam = FindFirstObjectByType<CamFollow2Players>();
        }
    }

  public int Register(PlayerScript_Multi pm)
{
    if (!pm) return -1;

    if (!players.Contains(pm))
        players.Add(pm);

    // which slot did this player take? (0 = P1, 1 = P2)
    int slot = Mathf.Clamp(players.IndexOf(pm), 0, 1);

    if (cam)
    {
        if (slot == 0) cam.player1 = pm.transform;
        else           cam.player2 = pm.transform;
    }
    return slot;
}

    public int Count => players.Count;

}
