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

            // Anunciamos a todos los clientes quién ganó
            // Llamada desde la StateAuthority a todos los clientes
            Rpc_AnnounceWinner(player);
        }
    }

    // RPC para anunciar el ganador a TODOS los clientes (incluido el servidor)
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_AnnounceWinner(PlayerRef winner, RpcInfo info = default)
    {
        string name = GetPlayerDisplayName(winner);

        Debug.Log($"Anunciando ganador: {name}");

        // Buscamos el componente VictoryUI en la escena local y le pedimos mostrar el canvas
        var victory = UnityEngine.Object.FindObjectOfType<VictoryUI>();
        if (victory != null)
        {
            victory.ShowWinner(name);
        }
        else
        {
            Debug.LogWarning("VictoryUI no encontrada en la escena. Agrega el prefab/canvas con VictoryUI.");
        }
    }

    private string GetPlayerDisplayName(PlayerRef p)
    {
        // Usar IsNone para detectar jugador "none"/host
        if (p.IsNone)
            return "Host";

        return $"Jugador {p.PlayerId}";
    }
}
