using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CheckpointScript : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
            if (player)
            {
                GameManager.Instance.NewCheckpointForThisPlayer(player.NetworkObject.OwnerClientId, this.transform);
            }
        }
    }
}
