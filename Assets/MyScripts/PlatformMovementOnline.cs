using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class PlatformMovementOnline : NetworkBehaviour
{
    public float Speed = 1.0f;
    public bool Active = false;

    [SerializeField] private Transform EndPointA;
    [SerializeField] private Transform EndPointB;
    [SerializeField] private Transform PlatformRoot;

    private Transform target = null;
    private Rigidbody playerRb = null;
    private Vector3 PlayerPositionCoef;
    private void Start()
    {
        target = EndPointA;
    }
    private void Update()
    {
        MoveTowardsTarget();
    }
    private void MoveTowardsTarget()
    {
        if (!Active) return;

        if (target != null)
        {
            // Calcule la nouvelle position
            Vector3 tamp = Vector3.MoveTowards(PlatformRoot.position, target.position, Speed * Time.deltaTime);
            Vector3 newPosition = new Vector3(tamp.x, PlatformRoot.position.y, tamp.z);
            Vector3 vecteurDirecteurDeplacement = (newPosition - PlatformRoot.position).normalized;
            float normeVecteurP1P2 = Vector3.Distance(newPosition, PlatformRoot.position);

            PlayerPositionCoef = normeVecteurP1P2*vecteurDirecteurDeplacement;
            PlatformRoot.position = newPosition;
        }
        if (playerRb)
        {
            playerRb.MovePosition(playerRb.position + PlayerPositionCoef);
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            playerRb = other.gameObject.GetComponent<Rigidbody>();
        }
        else if (other.gameObject == EndPointA.gameObject || other.gameObject == EndPointB.gameObject)
        {
            target = target == EndPointA ? EndPointB : EndPointA;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            playerRb = null;
        }
    }
    public void SetActive(bool active)
    {
        Active = active;
    }
}