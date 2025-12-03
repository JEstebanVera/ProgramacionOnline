using Fusion;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance;

    [Networked]
    public NetworkDictionary<PlayerRef, int> Scores => default;

    public event System.Action<PlayerRef> OnPlayerWin;

    public override void Spawned()
    {
        Instance = this;
        Debug.Log("ScoreManager Spawned correctamente.");
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_AddScore(PlayerRef player, int points)
    {
        int newScore = 0;

        if (!Scores.ContainsKey(player))
        {
            Scores.Set(player, points);
            newScore = points;
        }
        else
        {
            int current = Scores.Get(player);
            Scores.Set(player, current + points);
            newScore = current + points;
        }

        Debug.Log($"Nuevo puntaje para {player}: {Scores.Get(player)}");

        // VICTORIA 
        if (newScore >= 20)
        {
            OnPlayerWin?.Invoke(player);
        }
    }




}
