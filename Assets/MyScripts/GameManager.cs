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
    public NetworkVariable<int> TimeLeft = new NetworkVariable<int>(60 * 3);
    public List<ClientsInfos> ClientsInfos = new List<ClientsInfos>();

    private int _nbWinner = 0;
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

    public void InitClientsInfos()
    {
        if (IsServer)
        {
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds) 
            {
                ClientsInfos.Add(new ClientsInfos(clientId));
            }
        }
    }

    public IEnumerator RunGameStartCountdown()
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
        if (TimeLeft.Value <= 0) yield break;
        TimeLeft.Value--;

        if (TimeLeft.Value == 0)
        {
            // FIN DU JEU
            EndGame();
        }
        else
        {
            StartCoroutine(RunGameChrono());
        }
    }

    private void EndGame()
    {
        print("FIN DU JEU");
        /*foreach (var item in NetworkManager.Singleton.ConnectedClientsList)
        {
            item.PlayerObject.Despawn();
        }*/
        NetworkManager.Singleton.SceneManager.LoadScene("EndGame", LoadSceneMode.Single);
    }

    [ClientRpc]
    private void LetsGoPlayeClientRpc()
    {
        // Lancer tout les script avant debut de partie
        // ... TODO ....

        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerMovement>().EnableMovement = true;
    }

    public ClientsInfos GetPlayerInfo(ulong playerId)
    {
        foreach (var item in ClientsInfos)
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
        var player = GetPlayerInfo(playerId);
        player.ScoreTime = TimeLeft.Value;
        _nbWinner++;
        if (_nbWinner == 1)
        {
            TimeLeft.Value = 10;
        }
        if (_nbWinner == ClientsInfos.Count)
        {
            TimeLeft.Value = 0;
            // FIN DU JEU
            EndGame();
        }
    }

    public void NewCheckpointForThisPlayer(ulong playerId, Transform checkpoint)
    {
        print("PlayerId: " + playerId + " " + "Checkpoint: " + checkpoint);
        GetPlayerInfo(playerId).CheckpointPosition = checkpoint.position;
    }

}

[Serializable]
public class ClientsInfos
{
    public ulong ClientId { get; set; }
    public bool IsReady { get; set; }
    public int ScoreTime { get; set; }
    public Vector3 CheckpointPosition { get; set; }

    public ClientsInfos(ulong ClientId)
    {
        this.ClientId = ClientId;
        this.IsReady = false;
        this.ScoreTime = -1;
        //this.CheckpointPosition
        //this.Checkpoint = GameObject.FindGameObjectsWithTag("Spawn")[ClientId].transform;
    }
}