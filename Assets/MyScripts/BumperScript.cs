using Unity.Netcode;
using UnityEngine;

public class BumperScript : MonoBehaviour
{
    public float ForceBumper = 15f;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                collision.rigidbody.AddForce(-collision.contacts[0].normal * ForceBumper, ForceMode.Impulse);
            }
        }
    }
}
