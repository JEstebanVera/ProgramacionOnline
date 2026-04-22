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
        // Pequeño delay para asegurar que ScoreUI ya existe en todos los clientes
        StartActivateWithDelay();
    }

    private async void StartActivateWithDelay()
    {
        // Esperar 2 frames para que el cliente termine de inicializar
        await System.Threading.Tasks.Task.Delay(200);
        Rpc_ActivateScoreUI();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_ActivateScoreUI()
    {
        if (ScoreUI.Instance != null)
        {
            ScoreUI.Instance.Activate();
        }
        else
        {
            // Si aún no está listo, reintentamos
            Debug.LogWarning("ScoreUI.Instance es null, reintentando...");
            RetryActivate();
        }
    }

    private async void RetryActivate()
    {
        for (int i = 0; i < 10; i++)
        {
            await System.Threading.Tasks.Task.Delay(300);
            if (ScoreUI.Instance != null)
            {
                ScoreUI.Instance.Activate();
                return;
            }
        }
        Debug.LogError("ScoreUI nunca fue encontrado después de reintentos.");
    }

    // Sources.All para que el cliente también pueda llamarlo
    // pero la lógica solo corre en StateAuthority
    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void Rpc_AddScore(PlayerRef player, int points)
    {
        if (!HasStateAuthority) return;

        int current = 0;
        if (Scores.ContainsKey(player))
            current = Scores.Get(player);

        int newScore = current + points;
        Scores.Set(player, newScore);

        Debug.Log($"Score actualizado: Jugador {player.PlayerId} → {newScore}");

        if (newScore >= 20)
        {
            // Pasar scores de AMBOS jugadores directamente en el RPC
            // para evitar problemas de sincronización en el client
            int winnerScore = newScore;
            int loserScore = 0;

            foreach (var kvp in Scores)
            {
                if (kvp.Key != player)
                {
                    loserScore = kvp.Value;
                    break;
                }
            }

            Rpc_AnnounceWinner(player, winnerScore, loserScore);
        }
    }

    // Pasamos los scores directamente para garantizar que el client los recibe correctos
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_AnnounceWinner(PlayerRef winner, int winnerScore, int loserScore)
    {
        var victory = VictoryUI.Instance;
        if (victory != null)
        {
            victory.ShowWinner($"Jugador {winner.PlayerId}");
        }

        bool iWon = Runner.LocalPlayer == winner;

        // El score correcto: si gané soy el winner, si perdí soy el loser
        int myScore = iWon ? winnerScore : loserScore;

        Debug.Log($"EndMatch → iWon: {iWon}, myScore: {myScore}");

        PlayfabManager._PlayfabManager.EndMatch(iWon, myScore);
    }
}