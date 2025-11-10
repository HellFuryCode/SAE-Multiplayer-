using UnityEngine;
using Unity.Netcode;

[DisallowMultipleComponent]
public class HealthRespawnNet : NetworkBehaviour
{
    [Header("Health")]
    public int maxHP = 100;
    public float respawnDelay = 1.25f;

    [Header("Invulnerability")]
    public float invulnAfterRespawn = 1.0f;

    private int hp;
    private bool invulnerable = false;
    private PlayerRespawn respawn;

    void Awake()
    {
        respawn = GetComponent<PlayerRespawn>();
        hp = maxHP;
    }

    public void Damage(int amount)
    {
        if (amount <= 0) return;

        // If net active, only server mutates hp:
        if (IsNetActive() && !IsServer)
        {
            DamageServerRpc(amount);
            return;
        }

        ApplyDamage(amount);
    }

    bool IsNetActive() =>
        NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening;

    [ServerRpc(RequireOwnership = false)]
    void DamageServerRpc(int amount) => ApplyDamage(amount);

    void ApplyDamage(int amount)
    {
        if (invulnerable) return;

        hp = Mathf.Max(0, hp - amount);
        if (hp == 0)
        {
            // Death → respawn after delay on server
            Invoke(nameof(RespawnNow), respawnDelay);
        }
    }

    void RespawnNow()
    {
        hp = maxHP;
        if (respawn) respawn.RespawnNow();

        if (invulnAfterRespawn > 0)
        {
            invulnerable = true;
            Invoke(nameof(EndInvuln), invulnAfterRespawn);
        }

    }

    void EndInvuln() => invulnerable = false;
}
