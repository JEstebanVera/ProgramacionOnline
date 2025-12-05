using System.Collections;
using UnityEngine;
using TMPro;

public class VictoryUI : MonoBehaviour
{
    public static VictoryUI Instance;

    [Header("Referencias UI")]
    [SerializeField] private GameObject canvasRoot; // El GameObject del canvas (poner desactivado por defecto)
    [SerializeField] private TMP_Text winnerText;   // El TMP_Text que mostrará "Victoria para jugador [nombre]"

    private void Awake()
    {
        Instance = this;
        if (canvasRoot != null)
            canvasRoot.SetActive(false); // el canvas esté oculto al inicio
    }

    /// <summary>
    /// Mostrar el canvas con el nombre del ganador y cerrar después de 5s.
    /// Llamado por el RPC del ScoreManager en todos los clientes.
    /// </summary>
    public void ShowWinner(string playerDisplayName)
    {
        if (canvasRoot == null || winnerText == null)
        {
            Debug.LogError("VictoryUI: canvasRoot o winnerText no están asignados.");
            return;
        }

        winnerText.text = $"Victoria para jugador {playerDisplayName}";
        canvasRoot.SetActive(true);

        // iniciar corutina que cerrará el juego después de 5 segundos
        StopAllCoroutines();
        StartCoroutine(CloseAfterSeconds(5f));
    }

    private IEnumerator CloseAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
