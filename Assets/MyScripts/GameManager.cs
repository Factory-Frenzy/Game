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
    public NetworkVariable<int> GameStartCountdown = new NetworkVariable<int>(3);
    public NetworkVariable<int> TimeLeft = new NetworkVariable<int>(60*1+10);

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

    public void GetClientsInfos()
    {
        if (IsServer)
        {
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds) 
            {
                _clientsInfos.Add(new ClientsInfos(clientId));
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
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
            StartCoroutine(RunGameChrono());
        }
        else
        {
            StartCoroutine(RunGameStartCountdown());
        }
    }

    private IEnumerator RunGameChrono()
    {
        yield return new WaitForSeconds(1);
        TimeLeft.Value--;

        if (TimeLeft.Value == 0)
        {
            // FIN DU JEUX
            // TODO
        }
        else
        {
            StartCoroutine(RunGameChrono());
        }
    }

    [ClientRpc]
    private void LetsGoPlayeClientRpc()
    {
        // Lancer tout les script avant debut de partie
        // ... TODO ....

        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerMovement>().DisableMovement = false;
    }

    private ClientsInfos GetPlayerInfo(ulong playerId)
    {
        foreach (var item in _clientsInfos)
        {
            if (item.ClientId == playerId)
            {
                return item;
            }
        }
        Debug.LogError("Player not found ...");
        return null;
    }

    public void EndGameForThisPlayer(ulong playerId)
    {
        print("PlayerId: " + playerId + " " + "ScoreTime: " + TimeLeft.Value);
        GetPlayerInfo(playerId).ScoreTime = TimeLeft.Value;
    }

    public void NewCheckpointForThisPlayer(ulong playerId, Transform checkpoint)
    {
        print("PlayerId: " + playerId + " " + "Checkpoint: " + checkpoint);
        GetPlayerInfo(playerId).Checkpoint = checkpoint;
    }

}

public class ClientsInfos
{
    public ulong ClientId;
    public bool IsReady;
    public int ScoreTime;
    public Transform Checkpoint;

    public ClientsInfos(ulong ClientId)
    {
        this.ClientId = ClientId;
        this.IsReady = false;
        this.ScoreTime = -1;
        this.Checkpoint = null;
        //this.Checkpoint = GameObject.FindGameObjectsWithTag("Spawn")[ClientId].transform;
    }
}