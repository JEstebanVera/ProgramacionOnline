using Fusion;
using UnityEngine;

public abstract class Weapon : NetworkBehaviour
{
    [SerializeField] protected ShootType type;
    [SerializeField] protected Transform shootPoint;

    [Header("Prefabs")]
    [SerializeField] protected NetworkPrefabRef bullet; // PREFAB del proyectil 

    [SerializeField] protected int damage = 10;
    [SerializeField] protected float range = 100f;
    [SerializeField] protected int actualAmmo = 10;
    [SerializeField] protected Camera PlayerCam;
    [SerializeField] protected LayerMask layerMask;

    public abstract void RigidBodyShoot();
    public abstract void RpcRaycastShoot(RpcInfo info = default);
}
