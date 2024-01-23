using UnityEngine;

public class AccelerationCalculator : MonoBehaviour
{
    private float initialVelocityY;
    private float previousVelocityY;
    private float accelerationY;
    private Rigidbody rb = null;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Assurez-vous que l'objet a un Rigidbody attaché pour accéder à sa vélocité.
        if (rb != null)
        {
            // Obtenez la vélocité initiale en Y.
            initialVelocityY = rb.velocity.y;
            // La vélocité précédente commence par la vélocité initiale.
            previousVelocityY = initialVelocityY;
        }
        else
        {
            Debug.LogError("Rigidbody non trouvé sur cet objet !");
        }
    }

    void Update()
    {
        // Assurez-vous que l'objet a un Rigidbody attaché.
        if (rb != null)
        {
            // Obtenez la vélocité actuelle en Y.
            float currentVelocityY = rb.velocity.y;

            // Calculez l'accélération en Y en utilisant la différence de vélocité.
            accelerationY = (currentVelocityY - previousVelocityY) / Time.deltaTime;

            // Mettez à jour la vélocité précédente pour la prochaine itération.
            previousVelocityY = currentVelocityY;

            // Affichez l'accélération en Y dans la console.
            Debug.Log("Accélération en Y : " + accelerationY);
        }
    }
}
