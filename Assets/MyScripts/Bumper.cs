using UnityEngine;

public class Bumper : MonoBehaviour
{
    public float forceMagnitude = 10f; // La magnitude de la force à appliquer

    private void OnCollisionEnter(Collision collision)
    {
        // Vérifier si l'objet en collision est le joueur
        if (collision.gameObject.CompareTag("Player"))
        {
            // Obtenir le point de contact
            ContactPoint contact = collision.contacts[0];

            // Calculer la direction de la force : direction opposée au point de contact
            Vector3 forceDirection = contact.point - transform.position;
            forceDirection = -forceDirection.normalized;

            // Appliquer la force au joueur
            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                playerRigidbody.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
            }
        }
    }
}
