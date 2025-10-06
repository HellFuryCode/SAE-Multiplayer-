using UnityEngine;
using Unity.Netcode.Components;

[DisallowMultipleComponent]
public class ClientNetwrokAnimator : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false; //for animations for sinking so add this script to sink the aniamtions for enemies, players etc.
    }
}
