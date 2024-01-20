using Unity.Netcode;
using UnityEngine;

public class PlatformMovement : NetworkBehaviour
{
    [SerializeField] private Transform EndPointA;
    [SerializeField] private Transform EndPointB;
    [SerializeField] private Transform PlatformRoot;

    private Transform target;
    public float Speed = 1.0f;
    void Start()
    {
        target = EndPointA;
    }
    void Update()
    {
        print("coucou");
        MoveTowardsTarget();
    }
    private void MoveTowardsTarget()
    {
        if (target != null)
        {
            // Calcule la nouvelle position
            Vector3 newPosition = Vector3.MoveTowards(PlatformRoot.position, target.position, Speed * Time.deltaTime);
            // Déplace le GameObject vers la nouvelle position
            PlatformRoot.position = newPosition;
        }

        // Vérifie si le GameObject a atteint la cible
        if (Vector3.Distance(PlatformRoot.position, target.position) < 0.001f)
        {
            // Alterne la cible
            target = target == EndPointA ? EndPointB : EndPointA;
        }
    }
}
