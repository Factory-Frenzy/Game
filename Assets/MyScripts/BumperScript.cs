using Unity.Netcode;
using UnityEngine;

public class BumperScript : NetworkBehaviour
{
    public float ForceBumper = 15f;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && IsServer)
        {
            collision.rigidbody.AddForce(-collision.contacts[0].normal * ForceBumper, ForceMode.Impulse);
        }
    }
}
