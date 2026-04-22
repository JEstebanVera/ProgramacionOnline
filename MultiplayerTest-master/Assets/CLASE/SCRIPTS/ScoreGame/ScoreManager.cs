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

        // Activar ScoreUI para TODOS (host y client) cuando el ScoreManager spawnea.
        Rpc_ActivateScoreUI();
    }

    // activa el ScoreUI en todos los clientes
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_ActivateScoreUI()
    {
        if (ScoreUI.Instance != null)
        {
            ScoreUI.Instance.Activate();
        }
        else
        {
            Debug.LogWarning("ScoreUI.Instance es null cuando ScoreManager intentó activarlo.");
        }
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
        // Mostrar VictoryUI
        var victory = VictoryUI.Instance;
        if (victory != null)
        {
            victory.ShowWinner($"Jugador {winner.PlayerId}");
        }

        // Obtener el puntaje real de ESTE cliente local desde el NetworkDictionary
        int totalMatchScore = 0;
        if (Scores.ContainsKey(Runner.LocalPlayer))
        {
            totalMatchScore = Scores.Get(Runner.LocalPlayer);
        }

        bool iWon = Runner.LocalPlayer == winner;

        // Llamar EndMatch con el puntaje correcto de este cliente
        PlayfabManager._PlayfabManager.EndMatch(iWon, totalMatchScore);
    }
}