using Fusion;
using System;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private Transform viewportContent;
    [SerializeField] private GameObject lobbyPrefab;
    private void Start()
    {
        PhotonManager._PhotonManager.onSessionListUpdated += DestroyCanvasContent;
        PhotonManager._PhotonManager.onSessionListUpdated += UpdateSessionCanvas;
    }

    public void DestroyCanvasContent()
    {
        for (int i = viewportContent.childCount - 1; i >= 0; i--)
        {
            // no conocia el getchild pero creo q asi queda 
            Destroy(viewportContent.GetChild(i).gameObject);
        }
    }

    public void UpdateSessionCanvas()
    {
        DestroyCanvasContent();

        foreach(SessionInfo session in PhotonManager._PhotonManager.availableSessions)
        {
            GameObject sessionInstance = Instantiate(lobbyPrefab, viewportContent);
            sessionInstance.GetComponent<SessionEntry>().SetInfo(session);
        }
    }
}
