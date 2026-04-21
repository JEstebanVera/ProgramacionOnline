using Fusion;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance;

    [Networked]
    public NetworkDictionary<PlayerRef, int> Scores { get; }

    public event System.Action<PlayerRef> OnPlayerWin;

    public override void Spawned()
    {
        Instance = this;
        Debug.Log("Spawned ScoreManager - IsServer: " + Runner.IsServer);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void Rpc_AddScore(PlayerRef player, int points)
    {
        if (!HasStateAuthority)
            return;

        int current = 0;

        if (Scores.ContainsKey(player))
            current = Scores.Get(player);

        int newScore = current + points;
        Scores.Set(player, newScore);

        if (newScore >= 20)
        {
            Rpc_AnnounceWinner(player);
        }
    }

[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
private void Rpc_AnnounceWinner(PlayerRef winner)
{
    var victory = VictoryUI.Instance;
    if (victory != null)
    {
        // Usamos el ID para evitar nulos si no hay sistema de nombres
        victory.ShowWinner($"Jugador {winner.PlayerId}");
    }

    // OBTENER PUNTAJE REAL DESDE EL SERVER
    int totalMatchScore = 0;
    if (Scores.ContainsKey(Runner.LocalPlayer))
    {
        totalMatchScore = Scores.Get(Runner.LocalPlayer);
    }

    bool iWon = Runner.LocalPlayer == winner;

    // Enviamos el puntaje real que el servidor registró para este jugador
    PlayfabManager._PlayfabManager.EndMatch(iWon, totalMatchScore);
}

    private string GetPlayerDisplayName(PlayerRef p)
    {
        return $"Jugador {p.PlayerId}";
    }
}