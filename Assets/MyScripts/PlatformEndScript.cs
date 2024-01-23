using System;
using Unity.Netcode;
using UnityEngine;

public class PlatformEndScript : NetworkBehaviour
{
    public event EventHandler EndGameForMyPlayer;
    private void OnTriggerExit(Collider other)
    {
        PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
        if (player)
        {
            if (IsServer)
                GameManager.Instance.EndGameForThisPlayer(player.NetworkObject.OwnerClientId);
                

            if(player.NetworkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                player.EnableMovement = false;
                EndGameForMyPlayer?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
