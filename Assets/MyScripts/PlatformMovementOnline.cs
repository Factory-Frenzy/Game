using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatformMovementOnline : NetworkBehaviour
{
    public float Speed = 1.0f;

    [SerializeField] private Transform EndPointA;
    [SerializeField] private Transform EndPointB;
    [SerializeField] private Transform PlatformRoot;

    private Transform target = null;
    private List<Rigidbody> playersRb = new List<Rigidbody>();

    private void Start()
    {
        target = EndPointA;
    }
    private void FixedUpdate()
    {
        if (IsServer)
            MoveTowardsTarget();
    }
    private void MoveTowardsTarget()
    {
        if (target != null)
        {
            // Calcule la nouvelle position
            Vector3 tamp = Vector3.MoveTowards(PlatformRoot.position, target.position, Speed * Time.deltaTime);
            //Vector3 newPosition = new Vector3(tamp.x, PlatformRoot.position.y, tamp.z);
            Vector3 newPosition = tamp;
            Vector3 vecteurDirecteurDeplacement = (newPosition - PlatformRoot.position).normalized;
            float normeVecteurP1P2 = Vector3.Distance(newPosition, PlatformRoot.position);
            PlatformRoot.position = newPosition;

            foreach (var playerRb in playersRb)
            {
                playerRb.MovePosition(playerRb.position + normeVecteurP1P2 * vecteurDirecteurDeplacement);
            }
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
}