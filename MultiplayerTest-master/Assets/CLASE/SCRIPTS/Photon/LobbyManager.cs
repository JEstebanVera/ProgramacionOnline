using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    private void Start()
    {
        PhotonManager._PhotonManager.onSessionListUpdated += DestroyCanvasContent;
        PhotonManager._PhotonManager.onSessionListUpdated += UpdateSessionCanvas;
    }

    public void DestroyCanvasContent()
    {

    }

    public void UpdateSessionCanvas()
    {

    }
}
