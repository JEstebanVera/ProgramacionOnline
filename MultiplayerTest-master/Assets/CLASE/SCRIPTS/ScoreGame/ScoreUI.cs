using Fusion;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text myScoreText;
    [SerializeField] private TMP_Text enemyScoreText;

    private PlayerRef localPlayer;

    private void Start()
    {
        localPlayer = FindFirstObjectByType<NetworkRunner>().LocalPlayer;
    }

    private void LateUpdate()
    {
        if (ScoreManager.Instance == null)
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