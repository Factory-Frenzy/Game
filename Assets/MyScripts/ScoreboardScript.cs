using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TreeEditor;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ScoreboardScript : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _UIScoreboard;
    private List<ClientsInfos> _clientsInfos;
    private JsonSerializerSettings jsonSettings = new JsonSerializerSettings
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore
    };
    private void Start()
    {
        if (!IsServer) return;

        _clientsInfos = GameManager.Instance.ClientsInfos;
        _clientsInfos = _clientsInfos.OrderByDescending(client => client.ScoreTime).ToList();
        string clientsInfosString = JsonConvert.SerializeObject(_clientsInfos, jsonSettings);
        SendClientsInfosClientRpc(clientsInfosString);
    }

    [ClientRpc]
    private void SendClientsInfosClientRpc(string clientsInfosString)
    {
        _clientsInfos = JsonConvert.DeserializeObject<List<ClientsInfos>>(clientsInfosString);
        DispScoreBoard();
    }
    private void DispScoreBoard()
    {
        for (int i = 0; i < _clientsInfos.Count; i++)
        {
            var isMe = NetworkManager.Singleton.LocalClientId == _clientsInfos[i].ClientId ? "(You)" : "";
            if (_clientsInfos[i].ScoreTime > -1)
            {
                _UIScoreboard.text += i + 1 + ". Player: " + _clientsInfos[i].ClientId + isMe + " ScoreTime: " + _clientsInfos[i].ScoreTime + "\n";
            }
            else
            {
                _UIScoreboard.text += "X. Player: " + _clientsInfos[i].ClientId + isMe + " ScoreTime: Lose\n";
            }
        }
    }
}
