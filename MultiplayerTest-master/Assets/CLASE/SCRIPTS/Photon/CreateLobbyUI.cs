using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Fusion;

public class CreateLobbyUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInput;
    [SerializeField] private TMP_InputField maxPlayersInput;
    [SerializeField] private GameObject maxPlayersWarning;

    [SerializeField] private GameObject canvas1;
    [SerializeField] private GameObject canvas2;
    public async void CreateLobby()
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
            return; 
        }
        else
        {
            maxPlayersWarning.SetActive(false);
        }


        bool created = await PhotonManager._PhotonManager
            .CreateCustomLobby(lobbyName, maxPlayers);

        if (created)
        {
            canvas1.SetActive(false);
            canvas2.SetActive(false);
        }
    }

}

