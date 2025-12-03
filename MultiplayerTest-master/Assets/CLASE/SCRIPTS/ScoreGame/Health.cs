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

    private async void OnDeath(PlayerRef shooter)
    {
        // Esperar hasta que ScoreManager aparezca pq si no no jala
        int timeout = 0;
        while (ScoreManager.Instance == null && timeout < 50)
        {
            await Task.Delay(50);
            timeout++;
        }

        if (ScoreManager.Instance == null)
        {
            Debug.LogError("ScoreManager sigue siendo NULL después de esperar. Algo está mal.");
            return;
        }

        Debug.Log("ScoreManager listo, sumando puntos...");

        // Llamar al RPC del servidor
        ScoreManager.Instance.Rpc_AddScore(shooter, 1);

        // el evento OnDeath que usa balloon
        OnDeathEvent?.Invoke(shooter);
    }


}
