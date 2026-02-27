using Fusion;
using TMPro;
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

    [Header("Custom Session")]
    [SerializeField] private TMP_InputField sessionNameInput;

    private void OnEnable()
    {
        PhotonManager._PhotonManager.onSessionListUpdated += DestroyCanvasContent;
        PhotonManager._PhotonManager.onSessionListUpdated += UpdateSessionCanvas;


    }

    private void OnDisable()
    {
        PhotonManager._PhotonManager.onSessionListUpdated -= DestroyCanvasContent;
        PhotonManager._PhotonManager.onSessionListUpdated -= UpdateSessionCanvas;
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

    public void DestroyCanvasContent()
    {
        Debug.Log("Destoy Canvas");
        // Warning message
        warningMessage.SetActive(PhotonManager._PhotonManager.availableSessions.Count <= 0); 
        for (int i = 0; i < viewportContent.childCount; i++) 
        {
            Destroy(viewportContent.GetChild(i).gameObject);
        }
    }

}
