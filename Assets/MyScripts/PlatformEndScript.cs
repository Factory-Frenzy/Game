using UnityEngine;

public class PlatformEndScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
        if (player)
        {
            GameManager.Instance.EndGameForThisPlayer(player.NetworkObject.OwnerClientId);
        }
    }
}
