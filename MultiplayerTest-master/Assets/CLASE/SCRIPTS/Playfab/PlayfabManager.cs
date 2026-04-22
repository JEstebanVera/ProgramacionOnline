using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System.Threading.Tasks;
using System.Collections;
using System;
using PlayFab.AuthenticationModels;
using PlayFab.CloudScriptModels;
using System.Collections.Generic;

public class PlayfabManager : MonoBehaviour
{
    [Header("Login Inputs")]
    [SerializeField] private TMP_InputField loginUsernameInput;
    [SerializeField] private TMP_InputField loginPasswordInput;

    [Header("Register Inputs")]
    [SerializeField] private TMP_InputField registerEmailInput;
    [SerializeField] private TMP_InputField registerUsernameInput;
    [SerializeField] private TMP_InputField registerPasswordInput;

    [SerializeField] private GameObject IniciarCanvas;
    [SerializeField] private GameObject RegistrarCanvas;

    // Variables de 3 datos para la tarea
    public int totalBalloons;
    public int totalVictories;
    public int totalDefeats;

    private const string BALLOONS_KEY = "TotalBalloons";
    private const string VICTORIES_KEY = "Victories";
    private const string DEFEATS_KEY = "Defeats";

    // para mostrar los datos en el canvas
    [SerializeField] private TMP_Text balloonsText;
    [SerializeField] private TMP_Text victoriesText;
    [SerializeField] private TMP_Text defeatsText;

    //Esto hace que funcione lo de las skins
    public Dictionary<string, UserDataRecord> playerData { get; private set; }

    public static PlayfabManager _PlayfabManager;

    private void Awake()
    {
        if (_PlayfabManager == null)
        {
            _PlayfabManager = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

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
            await RegisterPlayfabAccount();

            Debug.Log("Usuario registrado correctamente");

            // Auto login usando los datos de registro
            loginUsernameInput.text = registerUsernameInput.text;
            loginPasswordInput.text = registerPasswordInput.text;

            await LoginWithPlayfab();

            RegistrarCanvas.SetActive(false);
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
            Username = registerUsernameInput.text.ToLower(),
            DisplayName = registerUsernameInput.text,
            Password = registerPasswordInput.text,
            Email = registerEmailInput.text,
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

            // Cargar estadísticas después del login
            await LoadStatistics();
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
            Username = loginUsernameInput.text.ToLower(),
            Password = loginPasswordInput.text,
        };

        // Descubrir como deben realizar la llamada a la API para que se pueda poner un await
        PlayFabClientAPI.LoginWithPlayFab(request, resultCallback => taskSource.SetResult(resultCallback), errorCallback => taskSource.SetException(new System.Exception(errorCallback.GenerateErrorReport())));

        return await taskSource.Task;
    }

    // Metodo para cargar datos al playfab

    public async Task LoadStatistics()
    {
        var result = await GetData();

        playerData = result.Data;

        if (playerData != null)
        {
            if (playerData.ContainsKey(BALLOONS_KEY))
                totalBalloons = int.Parse(playerData[BALLOONS_KEY].Value);

            if (playerData.ContainsKey(VICTORIES_KEY))
                totalVictories = int.Parse(playerData[VICTORIES_KEY].Value);

            if (playerData.ContainsKey(DEFEATS_KEY))
                totalDefeats = int.Parse(playerData[DEFEATS_KEY].Value);
        }

        UpdateStatsUI();
    }

    private void UpdateStatsUI()
    {
        if (balloonsText != null)
            balloonsText.text = $"Globos: {totalBalloons}";

        if (victoriesText != null)
            victoriesText.text = $"Victorias: {totalVictories}";

        if (defeatsText != null)
            defeatsText.text = $"Derrotas: {totalDefeats}";
    }

    // EndMatch ahora recarga los datos frescos de PlayFab ANTES de sumar,
    // para evitar sobreescribir con valores locales desactualizados.
    public async void EndMatch(bool iWon, int balloonsFromNetwork)
    {
        try
        {
            // Recargar datos actualizados desde PlayFab antes de modificar
            await LoadStatistics();

            // Sumar sobre los datos frescos
            totalBalloons += balloonsFromNetwork;

            if (iWon)
                totalVictories++;
            else
                totalDefeats++;

            SaveStatistics();

            // Refrescar la UI con los nuevos valores
            UpdateStatsUI();

            Debug.Log($"EndMatch guardado — Globos: {totalBalloons}, Victorias: {totalVictories}, Derrotas: {totalDefeats}");
        }
        catch (System.Exception e)
        {
            Debug.LogError("EndMatch falló: " + e.Message);
        }
    }

    // método para guardar los datos al playfab
    public void SaveStatistics()
    {
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
        {
            {BALLOONS_KEY, totalBalloons.ToString()},
            {VICTORIES_KEY, totalVictories.ToString()},
            {DEFEATS_KEY, totalDefeats.ToString()}
        }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("Estadísticas guardadas"),
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    // Metodo para obtener datos al playfab
    public async void GetPlayerData(Action[] postGetUserDataCalls = null)
    {
        try
        {
            var result = await GetData();
            playerData = result.Data;

            // Esto permite que no truene si no mandas nada
            if (postGetUserDataCalls != null)
            {
                foreach (var call in postGetUserDataCalls)
                {
                    call();
                }
            }

            //  DEBUG (tecla P)
            if (playerData != null)
            {
                if (playerData.ContainsKey("PlayerTitle"))
                    Debug.Log("PlayerTitle: " + playerData["PlayerTitle"].Value);

                if (playerData.ContainsKey("PlayerLevel"))
                    Debug.Log("PlayerLevel: " + playerData["PlayerLevel"].Value);

                if (playerData.ContainsKey("PlayerHealth"))
                    Debug.Log("PlayerHealth: " + playerData["PlayerHealth"].Value);
            }
        }
        catch (Exception error)
        {
            Debug.Log(error.Message);
        }
    }

    public async Task<GetUserDataResult> GetData()
    {
        var taskSource = new TaskCompletionSource<GetUserDataResult>();

        var request = new GetUserDataRequest();

        PlayFabClientAPI.GetUserData(request,
            resultCallback => taskSource.SetResult(resultCallback),
            errorCallback => taskSource.SetException(new Exception(errorCallback.GenerateErrorReport()))
        );

        return await taskSource.Task;
    }

    public void UploadSkinData()
    {
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
        {
            {"Body", "0"},
            {"Head", "1"},
            {"Material", "2"}
        }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("Skin guardada"),
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

}
