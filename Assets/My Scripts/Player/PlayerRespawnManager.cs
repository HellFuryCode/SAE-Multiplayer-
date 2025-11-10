using UnityEngine;

public class PlayerRespawnManager : MonoBehaviour
{
    public Transform player1Spawn;
    public Transform player2Spawn;

    public Transform GetSpawnFOrIndex (int idx)
    {
        return (idx == 0) ? player1Spawn : player2Spawn;
        
    }

}
