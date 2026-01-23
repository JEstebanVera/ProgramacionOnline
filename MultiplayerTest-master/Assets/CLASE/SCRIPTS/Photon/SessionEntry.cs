using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Fusion;

public class SessionEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text sessionName;
    [SerializeField] private TMP_Text playerCount;
    [SerializeField] private Button joinButton;

    public void SetInfo(SessionInfo sessionInfo)
    {
        sessionName.text = sessionInfo.Name;
        playerCount.text = sessionInfo.PlayerCount.ToString()
            + "/" +
            sessionInfo.MaxPlayers.ToString();

        if (sessionInfo.PlayerCount >= sessionInfo.MaxPlayers)
        {
            joinButton.interactable = false;
        }
    }

}
