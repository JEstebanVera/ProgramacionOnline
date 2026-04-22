using Fusion;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance;

    [SerializeField] private TMP_Text myScoreText;
    [SerializeField] private TMP_Text enemyScoreText;
    [SerializeField] private GameObject canvasRoot;

    private PlayerRef localPlayer;
    private bool isReady = false;

    private void Awake()
    {
        Instance = this;
        if (canvasRoot != null)
            canvasRoot.SetActive(false);
    }

    public void Activate()
    {
        // Buscar el runner y obtener LocalPlayer
        var runner = FindFirstObjectByType<NetworkRunner>();
        if (runner == null)
        {
            Debug.LogError("ScoreUI.Activate: No se encontró NetworkRunner.");
            return;
        }

        localPlayer = runner.LocalPlayer;

        // Validar que LocalPlayer es válido (no None)
        if (localPlayer == PlayerRef.None)
        {
            Debug.LogWarning("ScoreUI.Activate: LocalPlayer es None, reintentando...");
            RetryActivate(runner);
            return;
        }

        isReady = true;

        if (canvasRoot != null)
            canvasRoot.SetActive(true);

        Debug.Log($"ScoreUI activado para LocalPlayer: {localPlayer.PlayerId}");
    }

    private async void RetryActivate(NetworkRunner runner)
    {
        for (int i = 0; i < 10; i++)
        {
            await System.Threading.Tasks.Task.Delay(300);

            if (runner == null) return;

            localPlayer = runner.LocalPlayer;

            if (localPlayer != PlayerRef.None)
            {
                isReady = true;
                if (canvasRoot != null)
                    canvasRoot.SetActive(true);

                Debug.Log($"ScoreUI activado (retry) para LocalPlayer: {localPlayer.PlayerId}");
                return;
            }
        }

        Debug.LogError("ScoreUI: LocalPlayer nunca fue válido.");
    }

    private void LateUpdate()
    {
        if (!isReady || ScoreManager.Instance == null) return;

        var scores = ScoreManager.Instance.Scores;

        // Mi score
        int myScore = 0;
        scores.TryGet(localPlayer, out myScore);
        myScoreText.text = $"Yo: {myScore}";

        // Score del rival (cualquier player que NO sea yo)
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