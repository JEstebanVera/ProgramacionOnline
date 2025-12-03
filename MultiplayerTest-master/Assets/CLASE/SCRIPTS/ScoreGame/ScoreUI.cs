using Fusion;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text myScoreText;
    [SerializeField] private TMP_Text enemyScoreText;

    private PlayerRef localPlayer;

    public void Initialize(NetworkRunner runner)
    {
        localPlayer = runner.LocalPlayer;
    }

    private void Update()
    {
        if (ScoreManager.Instance == null) return;

        var scores = ScoreManager.Instance.Scores;

        // Mi puntaje
        if (scores.TryGet(localPlayer, out int myScore))
            myScoreText.text = $"Yo: {myScore}";
        else
            myScoreText.text = "Yo: 0";

        // Puntaje del rival 
        enemyScoreText.text = "Rival: 0";
        foreach (var kvp in scores)
        {
            if (kvp.Key != localPlayer)
            {
                enemyScoreText.text = $"Rival: {kvp.Value}";
                break;
            }
        }
    }
}
