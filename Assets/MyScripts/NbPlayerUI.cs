using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class NbPlayerUI : MonoBehaviour
{
    private TextMeshProUGUI _nbPlayerUi;
    void Start()
    {
        _nbPlayerUi = this.GetComponent<TextMeshProUGUI>();
        ConnectionApprovalHandler.NbClientUpdate += OnNbClientUpdate;
    }

    private void OnNbClientUpdate(object sender, int numberOfPlayers)
    {
        _nbPlayerUi.text = "Nb Players: " + numberOfPlayers.ToString();
    }
}
