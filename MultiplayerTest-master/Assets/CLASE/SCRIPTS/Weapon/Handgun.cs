using Fusion;
using UnityEngine;

public class Handgun : Weapon
{
    /// <summary>
    /// Un RPC es un protocolo para mandar a llamar un metodo en diferentes clientes
    /// 
    /// RpcSources es quien lo manda a llamar
    /// Rpctargets es quien lo ejecuta
    /// </summary>

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public override void RpcRaycastShoot(RpcInfo info = default)
    {
        if (Physics.Raycast(PlayerCam.transform.position, PlayerCam.transform.forward, out RaycastHit hit, range, layerMask))
        {
            Debug.Log(hit.collider.name);

            if(hit.collider.TryGetComponent(out Health health))
            {
                health.Rpc_TakeDamage(damage, info.Source);
            }
            else
            {
                // agujero de bala
            }
        }
    }

    public override void RigidBodyShoot()
    {
        RpcPhysicShoot(shootPoint.position, shootPoint.rotation);
    }

    /// <summary>
    /// El de arriba es el cliente, el de abajo el host
    /// </summary>

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]

    private void RpcPhysicShoot(Vector3 pos, Quaternion rot, RpcInfo info = default)
    {
        if (bullet.IsValid)
        {
            NetworkObject bulletInstance = Runner.Spawn(bullet, pos, rot, info.Source);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(PlayerCam.transform.position, PlayerCam.transform.forward * range);
    }
}
