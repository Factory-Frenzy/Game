using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CheckpointScript : NetworkBehaviour
{
    [SerializeField]
    private MeshRenderer _checkpointValidated;
    [SerializeField]
    private Material _greenMaterial;
    private List<ulong> _savedList = new List<ulong>();
    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
            if (player)
            {
                if (!_savedList.Contains(player.NetworkObject.OwnerClientId))
                {
                    GameManager.Instance.NewCheckpointForThisPlayer(player.NetworkObject.OwnerClientId, this.transform);
                    GameManager.Instance.NewCheckpointForThisPlayer(player.NetworkObject.OwnerClientId, this.transform);
                    CheckpointOKServerRpc(player.NetworkObject.OwnerClientId);
                    _savedList.Add(player.NetworkObject.OwnerClientId);
                }
            }
        }
    }

    [ServerRpc]
    private void CheckpointOKServerRpc(ulong clientId)
    {
        CheckpointOKClientRpc(clientId);
    }

    [ClientRpc]
    private void CheckpointOKClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            _checkpointValidated.SetMaterials(new List<Material> { _greenMaterial });
        }
    }

}
