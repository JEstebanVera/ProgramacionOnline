using Fusion;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    // Singleton para que ScoreManager pueda encontrarlo fácilmente (espero)
    public static ScoreUI Instance;

    [SerializeField] private TMP_Text myScoreText;
    [SerializeField] private TMP_Text enemyScoreText;

    // El canvas raíz del ScoreUI (Se llama JuegoCanvas)
    [SerializeField] private GameObject canvasRoot;

    private PlayerRef localPlayer;
    private bool isReady = false;

    private void Awake()
    {
        Instance = this;

        // Va a estar apagado al principio
        if (canvasRoot != null)
            canvasRoot.SetActive(false);
    }

    /// <summary>
    /// Llamado por ScoreManager vía RPC en todos los clientes.
    /// Obtiene el LocalPlayer en este momento (cuando el runner ya está listo)
    /// y activa el canvas.
    /// </summary>
    public void Activate()
    {
        var runner = FindFirstObjectByType<NetworkRunner>();
        if (runner != null)
        {
            localPlayer = runner.LocalPlayer;
            isReady = true;
        }
        else
        {
            Debug.LogError("ScoreUI.Activate: No se encontró NetworkRunner.");
            return;
        }

        if (canvasRoot != null)
            canvasRoot.SetActive(true);

        Debug.Log($"ScoreUI activado para: {localPlayer}");
    }

    private void LateUpdate()
    {
        // No procesar si todavía no estamos listos
        if (!isReady || ScoreManager.Instance == null)
            return;

        var scores = ScoreManager.Instance.Scores;

        int myScore = 0;
        scores.TryGet(localPlayer, out myScore);
        myScoreText.text = $"Yo: {myScore}";

        int enemyScore = 0;
        foreach (var kvp in scores)
        {
            if (kvp.Key != localPlayer)
            {
                enemyScore = kvp.Value;
                break;
            }
        }
        enemyScoreText.text = $"Rival: {enemyScore}";
    }
}