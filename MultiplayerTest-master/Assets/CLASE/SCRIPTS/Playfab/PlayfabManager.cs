using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System.Threading.Tasks;
using System.Collections;
using System;
using PlayFab.AuthenticationModels;
using PlayFab.CloudScriptModels;

public class PlayfabManager : MonoBehaviour
{

    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;

    [SerializeField] private GameObject IniciarCanvas;

    // Variables de 3 datos para la tarea
    [SerializeField] private string playerTitle;
    [SerializeField] private int playerLevel;
    [SerializeField] private float playerHealth;

    private void Start()
    {
              
        if(PlayFabSettings.DeveloperSecretKey == null)
        {
            PlayFabSettings.DeveloperSecretKey = "FUZSSXYDK1EA1ETPECR95TT4QC3PQJEF8U956C81CM9HTHSAWC";
        }

        if (PlayFabSettings.TitleId == null)
        {
            PlayFabSettings.TitleId = "183321";
        }
    }

    /// <summary>
    /// Este metodo es el que va a ir en el boton registrar usuario
    /// </summary>
    public async void RegisterUser()
    {
        try
        {
            var result = await RegisterPlayfabAccount();
            Debug.Log("");
        }
        catch (Exception error) 
        {
            Debug.Log(error.Message);
        }
    }

    /// <summary>
    /// Los metodos que realizan las request reciben una respuesta
    /// </summary>
    /// <returns></returns>
    public async Task<RegisterPlayFabUserResult> RegisterPlayfabAccount()
    {

        var taskSource = new TaskCompletionSource<RegisterPlayFabUserResult>(); // Es un tipo de variable donde se almacenan resultado, y los devuelve

        RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest()
        {
            Username = usernameInput.text.ToLower(),
            DisplayName = usernameInput.text,
            Password = passwordInput.text,
        };

        // Descubrir como deben realizar la llamada a la API para que se pueda poner un await
        PlayFabClientAPI.RegisterPlayFabUser(request, resultCallback => taskSource.SetResult(resultCallback),errorCallback => taskSource.SetException(new System.Exception(errorCallback.GenerateErrorReport())));
    
        return await taskSource.Task;
    }

    public async void PlayfabLogin()
    {
        try
        {
            var result = await LoginWithPlayfab();

            Debug.Log("Login Successful");

            // cerrar el panel solo si login fue exitoso
            IniciarCanvas.SetActive(false);
        }
        catch(Exception error)  
        {
            Debug.Log(error.Message);
        }
    }

    public async Task<LoginResult> LoginWithPlayfab()
    {
        var taskSource = new TaskCompletionSource<LoginResult>(); // Es un tipo de variable donde se almacenan resultado, y los devuelve

        LoginWithPlayFabRequest request = new LoginWithPlayFabRequest()
        {
            Username = usernameInput.text.ToLower(),
            Password = passwordInput.text,
        };

        // Descubrir como deben realizar la llamada a la API para que se pueda poner un await
        PlayFabClientAPI.LoginWithPlayFab(request, resultCallback => taskSource.SetResult(resultCallback), errorCallback => taskSource.SetException(new System.Exception(errorCallback.GenerateErrorReport())));

        return await taskSource.Task;
    }

}
