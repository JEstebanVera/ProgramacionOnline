using Fusion;
using UnityEngine;

public class Balloon : NetworkBehaviour
{
    [SerializeField] private Health health;

    public override void Spawned()
    {
        if (health == null)
            health = GetComponent<Health>();
    }

    void Start()
    {
        // Cuando el globo muera, Health llamará a ScoreManager
        health.OnDeathEvent += OnBalloonDestroyed;
    }

    private void OnBalloonDestroyed(PlayerRef shooter)
    {
        if (Runner.IsServer)
        {
            Runner.Despawn(Object);   // Destruye el globo
        }
    }
}
