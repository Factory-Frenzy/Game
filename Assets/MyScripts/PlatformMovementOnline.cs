using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PlatformMovementOnline : NetworkBehaviour
{
    [SerializeField] private Transform EndPointA;
    [SerializeField] private Transform EndPointB;
    [SerializeField] private Transform PlatformRoot;

    private Rigidbody PlatformRootRb;
    private Transform target;
    private Rigidbody playerRb = null;
    private Vector3 vecteurDirecteurDeplacement;
    private float normeVecteurP1P2;
    private Vector3 newPosition;
    public float Speed = 1.0f;
    void Start()
    {
        target = EndPointA;
        PlatformRootRb = PlatformRoot.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
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
            if (IsServer)
                PlatformRoot.gameObject.GetComponent<Rigidbody>().position = newPosition;
        }

        // Vérifie si le GameObject a atteint la cible
        if (Vector3.Distance(PlatformRoot.gameObject.GetComponent<Rigidbody>().position, target.position) < 0.001f)
        {
            // Alterne la cible
            target = target == EndPointA ? EndPointB : EndPointA;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            if (other.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                playerRb = other.gameObject.GetComponent<Rigidbody>();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            if (other.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                playerRb = null;
            }
        }
    }
    private void MoveTowardsPlayer()
    {
        if (!playerRb) return;

        //print("Rb: " + playerRb.position + " P2: " + positionP2 + " delta: "+delta.normalized+" rayon: "+rayonCercle);
        playerRb.MovePosition(playerRb.position + normeVecteurP1P2 * vecteurDirecteurDeplacement);
    }
}
