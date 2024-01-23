using Unity.Netcode;
using UnityEngine;

public class PlatformTrampolineScript : NetworkBehaviour
{
    public float TrampoForce = 10f;
    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer && collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMovement>()
                .AnimationServerRpc(
                    collision.gameObject.GetComponent<NetworkObject>().OwnerClientId,
                    "Jump",
                    true
                );
            collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * TrampoForce,ForceMode.Impulse);
        }
    }
}
