using Fusion;
using UnityEngine;
using System.Threading.Tasks;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 1;

    [Networked]
    public int HP { get; set; }

    public event System.Action<PlayerRef> OnDeathEvent;


    public override void Spawned()
    {

        HP = maxHealth;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_TakeDamage(int damage, PlayerRef shooter)
    {
        HP -= damage;

        if (HP <= 0)
        {
            OnDeath(shooter);
        }
    }

    private void OnDeath(PlayerRef shooter)
    {
        if (!HasStateAuthority)
            return;

        ScoreManager.Instance.Rpc_AddScore(shooter, 1);

        OnDeathEvent?.Invoke(shooter);
    }


}
