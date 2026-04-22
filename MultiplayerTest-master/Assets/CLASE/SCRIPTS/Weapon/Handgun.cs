using Fusion;
using UnityEngine;

public class Handgun : Weapon
{
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public override void RpcRaycastShoot(RpcInfo info = default)
    {
        // Fix: info.Source es None cuando el host se llama a sí mismo
        PlayerRef shooter = info.Source.IsNone ? Object.InputAuthority : info.Source;

        if (Physics.Raycast(PlayerCam.transform.position, PlayerCam.transform.forward,
                            out RaycastHit hit, range, layerMask))
        {
            Debug.Log($"Hit: {hit.collider.name} | Shooter: {shooter}");
            if (hit.collider.TryGetComponent(out Health health))
            {
                health.Rpc_TakeDamage(damage, shooter);
            }
        }
    }

    public override void RigidBodyShoot()
    {
        RpcPhysicShoot(shootPoint.position, shootPoint.rotation);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RpcPhysicShoot(Vector3 pos, Quaternion rot, RpcInfo info = default)
    {
        // Fix: info.Source es None cuando el host se llama a sí mismo
        PlayerRef shooter = info.Source.IsNone ? Object.InputAuthority : info.Source;

        Debug.Log($"Disparo de {shooter.PlayerId}");

        if (bullet.IsValid)
        {
            Runner.Spawn(bullet, pos, rot, shooter);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(PlayerCam.transform.position, PlayerCam.transform.forward * range);
    }

}



