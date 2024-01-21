using Unity.Netcode;
using UnityEngine;

public class PlatformMovementOnline : NetworkBehaviour
{
    [SerializeField] private Transform EndPointA;
    [SerializeField] private Transform EndPointB;
    [SerializeField] private Transform PlatformRoot;

    private Transform target;
    private Rigidbody playerRb = null;
    private Vector3 newPosition;
    public float Speed = 1.0f;
    void Start()
    {
        target = EndPointA;
    }
    void Update()
    {
        MoveTowardsTarget();
        MoveTowardsPlayer();
    }
    private void MoveTowardsTarget()
    {
        if (target != null)
        {
            // Calcule la nouvelle position
            newPosition = Vector3.MoveTowards(PlatformRoot.position, target.position, Speed * Time.deltaTime);
            // Déplace le GameObject vers la nouvelle position
            if(IsServer)
            PlatformRoot.position = newPosition;
        }

        // Vérifie si le GameObject a atteint la cible
        if (Vector3.Distance(PlatformRoot.position, target.position) < 0.001f)
        {
            // Alterne la cible
            target = target == EndPointA ? EndPointB : EndPointA;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(other.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                playerRb = other.gameObject.GetComponent<Rigidbody>();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
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

        Vector3 playerMove = Vector3.MoveTowards(playerRb.position, target.position, Speed * Time.deltaTime);

        if (newPosition.normalized.magnitude == Vector3.right.magnitude)
        {
            playerMove = new Vector3(playerMove.x, playerRb.position.y, playerRb.position.z);
        }
        else if (newPosition.normalized.magnitude == Vector3.forward.magnitude)
        {
            playerMove = new Vector3(playerRb.position.x, playerRb.position.y, playerMove.z);
        }
        else
        {
            playerMove = new Vector3(playerRb.position.x, playerMove.y, playerRb.position.z);
        }

        playerRb.MovePosition(playerMove);
    }
}
