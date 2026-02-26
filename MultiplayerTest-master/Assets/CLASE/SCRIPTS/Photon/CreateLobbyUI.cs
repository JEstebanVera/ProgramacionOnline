using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Fusion;

public class CreateLobbyUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInput; // donde se pone el nombre del lobby
    [SerializeField] private TMP_InputField maxPlayersInput; // donde se pone la cantidad maxima de jugadores
    [SerializeField] private TMP_Text sessionName;
    [SerializeField] private TMP_Text playerCount;
    [SerializeField] private Button joinButton;
    [SerializeField] private GameObject maxPlayersWarning; // la advertencia si supera 10 jugadores
    public void CreateLobby()
    {
        string lobbyName = lobbyNameInput.text;

        if (string.IsNullOrEmpty(lobbyName))
        {
            Debug.LogWarning("El lobby necesita un nombre.");
            return;
        }

        int maxPlayers;

        if (!int.TryParse(maxPlayersInput.text, out maxPlayers))
        {
            Debug.LogWarning("Número de jugadores inválido.");
            return;
        }

        if (maxPlayers < 1)
        {
            maxPlayers = 1;
        }

        if (maxPlayers > 10)
        {
            Debug.LogWarning("No puedes crear una sala con más de 10 jugadores.");
            maxPlayersWarning.SetActive(true);
            return; // IMPORTANTE: NO crear lobby
        }
        else
        {
            maxPlayersWarning.SetActive(false);
        }

        PhotonManager._PhotonManager.CreateCustomLobby(lobbyName, maxPlayers);
    }


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

    public void JoinSession()
    {
        PhotonManager._PhotonManager.JoinSession(sessionName.text);
    }

}

