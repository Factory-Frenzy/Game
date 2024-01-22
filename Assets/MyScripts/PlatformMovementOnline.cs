using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PlatformMovementOnline : NetworkBehaviour
{
    public float Speed = 1.0f;

    [SerializeField] private Transform EndPointA;
    [SerializeField] private Transform EndPointB;
    [SerializeField] private Transform PlatformRootRb;

    private Transform target;
    private List<Rigidbody> playersRb = new List<Rigidbody>();
    private Vector3 vecteurDirecteurDeplacement;
    private float normeVecteurP1P2;
    private Vector3 newPosition;

    void Start()
    {
        target = EndPointA;
    }
    private void FixedUpdate()
    {
        if (!IsServer) return;
        MoveTowardsTarget();
        MoveTowardsPlayer();
    }
    private void MoveTowardsTarget()
    {
        if (target != null)
        {
            // Calcule la nouvelle position
            newPosition = Vector3.MoveTowards(PlatformRootRb.position, target.position, Speed * Time.deltaTime);
            vecteurDirecteurDeplacement = (newPosition - PlatformRootRb.position).normalized;
            normeVecteurP1P2 = Vector3.Distance(newPosition, PlatformRootRb.position);
            PlatformRootRb.position = newPosition;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playersRb.Add(other.gameObject.GetComponent<Rigidbody>());
        }
        else if (other.gameObject == EndPointA.gameObject || other.gameObject == EndPointB.gameObject)
        {
            target = target == EndPointA ? EndPointB : EndPointA;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playersRb.Remove(other.gameObject.GetComponent<Rigidbody>());
        }
    }
    private void MoveTowardsPlayer()
    {
        foreach (var playerRb in playersRb)
        {
            ulong clientId = playerRb.gameObject.GetComponent<NetworkObject>().OwnerClientId;
            MovePositionClientRpc(clientId, playerRb.position + normeVecteurP1P2 * vecteurDirecteurDeplacement);
        }
    }

    [ClientRpc]
    private void MovePositionClientRpc(ulong clientId, Vector3 position)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Rigidbody>().MovePosition(position);
        }
    }
}