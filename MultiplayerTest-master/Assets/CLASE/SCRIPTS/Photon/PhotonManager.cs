using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System.Threading.Tasks;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviour, INetworkRunnerCallbacks
{


    [SerializeField] private NetworkPrefabRef prefab; // Referencia al prefab
    [SerializeField] private NetworkRunner runner; // Runner es quien se encarga de enviar y recibir informacion, es tu medio de comunicacion con el servidor
    [SerializeField] NetworkSceneManagerDefault sceneManager;
    [SerializeField] private Transform[] spawnPoint;

    [SerializeField] Dictionary<PlayerRef, NetworkObject> players = new Dictionary<PlayerRef, NetworkObject>(); // PlayerRef es el ID de nuestro jugador en la red, NetwokrObject es el prefab/objeto de nuestro jugador
    [SerializeField] UnityEvent onPlayerJoinedToGame; // Los UnityEvents son llamadas que se hacen al invocar un evento

    public List<SessionInfo> availableSessions = new List<SessionInfo>(); // Sesiones disponibles 
    // Si esta lista tiene sesiones dentro, significa que hay sesiones disponibles, si esta vacia pues no hay sesiones

    public event Action onSessionListUpdated;
    public static PhotonManager _PhotonManager;
    public bool isHost;

    [SerializeField] private Button createLobbyButton;

    private void Awake()
    {
        if (_PhotonManager == null)
        {
            _PhotonManager = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        runner.AddCallbacks(this);
    }

    public async void JoinLobby()
    {
        await runner.JoinSessionLobby(SessionLobby.ClientServer);
        createLobbyButton.interactable = true;  
        Debug.Log("CONECTADO");
    }


    #region Metodos de Photon
    /// <summary>
    /// 
    /// Callback es algo que se manda a llamar automaticamente cuando otro proceso termina
    /// 
    /// Network Runner es tu instancia de la partida
    /// </summary>

    /// <summary>
    /// El siguiente paso es tener registrados que jugadores estan entrando
    /// </summary>
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            int randomSpawn = UnityEngine.Random.Range(0, spawnPoint.Length);
            NetworkObject networkPlayer = runner.Spawn(
                prefab,
                spawnPoint[randomSpawn].position,
                spawnPoint[randomSpawn].rotation,
                player  //Esto asigna InputAuthority correctamente
            );
            players.Add(player, networkPlayer);

            Debug.Log($"Spawneado jugador: {player.PlayerId} con InputAuthority: {networkPlayer.InputAuthority}");
        }
        onPlayerJoinedToGame.Invoke();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (players.TryGetValue(player, out NetworkObject networkPlayer)) // Si en mi diccionario existe a referencia a ese jugador, 
        {
            runner.Despawn(networkPlayer); // Elimino el objeto de el jugador de la escena
            players.Remove(player); // Lo elimino de mi diccionario
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // Creo un objeto de tipo NetworkInputData
        NetworkInputData data = new NetworkInputData()
        {
            move = InputManager.Instance.GetMoveInput() == null ? new Vector2(0, 0) : InputManager.Instance.GetMoveInput(),
            look = InputManager.Instance.GetMouseDelta(),
            isRunning = InputManager.Instance.WasRunInputPressed(),
            yRotation = Camera.main.transform.eulerAngles.y,
            shoot = InputManager.Instance.ShootInputPressed()
        };

        input.Set(data);
    }

    /// <summary>
    /// Este metodo se manda a llamar cada que un host inicia una nueva sesion
    /// 
    /// Esto solo actualiza tal cual una lista, pero List<> 
    /// 
    /// El parametro sessionList es una lista de sesiones, esta se actualiza cada vez que
    /// se crea o se elimina una sesion.
    /// </summary>
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("Sesiones activas: " + sessionList.Count);
        availableSessions = sessionList; // Aqui guardo la lista de sesiones más reciente
        onSessionListUpdated?.Invoke();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    #endregion


    /// <summary>
    /// En este metodo vamos a crear o buscar una partida. Si no existe alguna partida o lobby, entonces
    /// nosotros lo creamos y somos el host, si ya hay una partida entonces entramos y somos el cliente.
    /// 
    /// Aqui vamos a configurar, cuantos jugadores se pueden conectar a la partida como maximo, cual mapa va a ser el que va a cargar
    /// tambien si dentro de la partida puede haber cambios de escena.
    /// 
    /// SceneRef guarda que escena se va a usar
    /// NetworkSceneInfo guarda como se van a usar las escenas en mi juego
    /// Esta puede guardar la informacion de hasta 8 escenas
    /// </summary>
    private async void StartGame(GameMode mode)
    {
        runner.ProvideInput = true; // Esto nos dice que el runner recibira y mandara inputs
        var scene = SceneRef.FromIndex(0); // Guardame una referencia a la escena 0.
        var sceneInfo = new NetworkSceneInfo(); // Creo una variable que me va a guardar las escenas que voy a usar, 
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = RandomSessionName(6), // Este nombre es el interno que yo como desarrollador necesito entender
            Scene = scene,
            SceneManager = sceneManager,
            IsVisible = true,
        });
    }


    public async void JoinSession(string sessionName)
    {
        runner.ProvideInput = true; // Esto nos dice que el runner recibira y mandara inputs
        var scene = SceneRef.FromIndex(0); // Guardame una referencia a la escena 0.
        var sceneInfo = new NetworkSceneInfo(); // Creo una variable que me va a guardar las escenas que voy a usar, 
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = sessionName, // Este nombre es el interno que yo como desarrollador necesito entender
            Scene = scene,
            SceneManager = sceneManager,
            IsVisible = true
        });
    }

    public void StartGameAsHost()
    {
        StartGame(GameMode.Host);
    }

    public void StartGameAsClient()
    {
        StartGame(GameMode.Client);
    }

    private string RandomSessionName(int sessionNameLength)
    {
        string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        string sessionName = "";

        for(int i = 0; i < sessionNameLength; i++)
        {
            char randomChar = caracteres[UnityEngine.Random.Range(0,caracteres.Length)];
            sessionName += randomChar; // "" + a = "a" // "a" = "a" + "k" = "ak"
        }

        return sessionName;
    }

    public async Task<bool> CreateCustomLobby(string sessionName, int maxPlayers)
    {
        runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(0);

        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = sessionName,
            PlayerCount = maxPlayers,
            Scene = scene,
            SceneManager = sceneManager,
            IsVisible = true,
            IsOpen = true
        });

        return result.Ok;
    }


}
