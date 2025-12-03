using Fusion;
using UnityEngine;
using System.Threading.Tasks;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private int damage = 10;
    [SerializeField] private int lifeTime = 5; 

    private Rigidbody rb;

    // PlayerRef del que dispar� 
    private PlayerRef owner;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();

        // asignar owner a partir de la InputAuthority del objeto (cuando lo spawnearon con info.Source)
        owner = Object.InputAuthority;

        // establecer velocidad f�sica
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }

        // iniciar auto despawn
        DespawnAfterTime();
    }

    private async void DespawnAfterTime()
    {
        await Task.Delay(lifeTime * 1000);
        if (Object != null)
            Runner.Despawn(Object);
    }

   
    private void OnTriggerEnter(Collider other)
    {
        
        if (!HasStateAuthority)
            return;


        if (other.TryGetComponent<Health>(out var health))
        {
            // Llamamos al RPC para aplicar da�o 
            health.Rpc_TakeDamage(damage, owner);

            // despawn inmediatamente
            Runner.Despawn(Object);
        }
        else
        {

            Runner.Despawn(Object);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!HasStateAuthority)
            return;

        if (collision.collider.TryGetComponent<Health>(out var health))
        {
            health.Rpc_TakeDamage(damage, owner);
            Runner.Despawn(Object);
        }
        else
        {
            Runner.Despawn(Object);
        }
    }
}
