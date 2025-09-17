using UnityEditor.EditorTools;
using UnityEngine;

public class PlayerIdentity : MonoBehaviour
{
    //too make life simple 
    [Tooltip("0 = player 1, 1 =player2")]  //this shit is so cool i wish i knew this soooner
    public int playerIndex = 0;

    [Tooltip("shows the winner panel")]

    public string displayName = "player 1";
}
