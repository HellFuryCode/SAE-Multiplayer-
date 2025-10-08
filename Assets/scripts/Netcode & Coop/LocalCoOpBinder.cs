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

    public void Register(PlayerScript_Multi pm)
    {
        if (!pm || players.Contains(pm))
        {
            return;
        }

        players.Add(pm);

        if (cam)
        {
            if (players.Count >= 1)
            {
                cam.player1 = players[0].transform;
            }

            if (players.Count >= 2)
            {
                cam.player2 = players[1].transform;
            }
        }
    }

    public int Count => players.Count;

}
