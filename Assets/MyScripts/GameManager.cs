using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    [NonSerialized]
    public static GameManager Instance = null;
    public NetworkVariable<int> GameStartCountdown = new NetworkVariable<int>(10);

    private List<ClientsInfos> _clientsInfos = new List<ClientsInfos>();
    private int _nbClientsReady = 0;
    private readonly object _lock = new object();
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (IsServer)
        {
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds) 
            {
                _clientsInfos.Add(new ClientsInfos(clientId));
            }
        }
    }

    [ServerRpc]
    public void ImReadyServerRpc()
    {
        lock (_lock)
        {
            _nbClientsReady += 1;
            if (_clientsInfos.Count == _nbClientsReady)
            {
                StartCoroutine(RunGameStartCountdown());
            }
        }
    }

    private IEnumerator RunGameStartCountdown()
    {
        yield return new WaitForSeconds(1);
        GameStartCountdown.Value--;
        if (GameStartCountdown.Value == 0)
        {
            LetsGoPlayeClientRpc();
        }
        else
        {
            StartCoroutine(RunGameStartCountdown());
        }
    }

    [ClientRpc]
    private void LetsGoPlayeClientRpc()
    {
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerMovement>().DisableMovement = false;
    }
}

public class ClientsInfos
{
    public ulong ClientId;
    public bool IsReady;

    public ClientsInfos(ulong ClientId)
    {
        this.ClientId = ClientId;
        this.IsReady = false;
    }
}