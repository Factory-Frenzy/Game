using Unity.Netcode;
using UnityEngine;

public class PlatformEndScript : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
            if (player)
            {
                GameManager.Instance.EndGameForThisPlayer(player.NetworkObject.OwnerClientId);
            }
        }
    }
}