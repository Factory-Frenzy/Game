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
    private bool _saved = false;
    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
            if (player && !_saved)
            {
                GameManager.Instance.NewCheckpointForThisPlayer(player.NetworkObject.OwnerClientId, this.transform);
                _checkpointValidated.SetMaterials(new List<Material> { _greenMaterial });
                _saved = true;
            }
        }
    }
}
