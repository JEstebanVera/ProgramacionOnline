using Fusion;
using UnityEngine;



/// <summary>
/// Este script es el que va a actualizar mi lista de sesiones
/// en el canvas
/// </summary>
public class LobbyManager : MonoBehaviour
{

    [SerializeField] private Transform viewportContent;
    [SerializeField] private GameObject lobbyPrefab;
    [SerializeField] private GameObject warningMessage;

    private void OnEnable()
    {
        PhotonManager._PhotonManager.onSessionListUpdated += DestroyCanvasContent;
        PhotonManager._PhotonManager.onSessionListUpdated += UpdateSessionCanvas;
    }

    public void UpdateSessionCanvas()
    {
        Debug.Log("Creando sesiones: " + PhotonManager._PhotonManager.availableSessions.Count);
        foreach(SessionInfo session in PhotonManager._PhotonManager.availableSessions)
        {
            GameObject sessionInstance = Instantiate(lobbyPrefab,viewportContent);
            sessionInstance.GetComponent<SessionEntry>().SetInfo(session);
        }
    }

    // Aqui deben destruir el contenido de el viewportContent
    // Obligatorio: Usar for, foreach no
    public void DestroyCanvasContent()
    {
        Debug.Log("Destoy Canvas");
        warningMessage.SetActive(PhotonManager._PhotonManager.availableSessions.Count <= 0);

        for (int i = 0; i < viewportContent.childCount; i++) 
        {
            Destroy(viewportContent.GetChild(i).gameObject);
        }
    }

}
