using UnityEngine;
using TMPro;

public class CreateLobbyUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInput;
    [SerializeField] private TMP_InputField maxPlayersInput;

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

        if (maxPlayers > 10) // ajustar el límite
        {
            maxPlayers = 10;
        }
    }
}

