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
        // Assurez-vous que l'objet a un Rigidbody attach� pour acc�der � sa v�locit�.
        if (rb != null)
        {
            // Obtenez la v�locit� initiale en Y.
            initialVelocityY = rb.velocity.y;
            // La v�locit� pr�c�dente commence par la v�locit� initiale.
            previousVelocityY = initialVelocityY;
        }
        else
        {
            Debug.LogError("Rigidbody non trouv� sur cet objet !");
        }
    }

    void Update()
    {
        // Assurez-vous que l'objet a un Rigidbody attach�.
        if (rb != null)
        {
            // Obtenez la v�locit� actuelle en Y.
            float currentVelocityY = rb.velocity.y;

            // Calculez l'acc�l�ration en Y en utilisant la diff�rence de v�locit�.
            accelerationY = (currentVelocityY - previousVelocityY) / Time.deltaTime;

            // Mettez � jour la v�locit� pr�c�dente pour la prochaine it�ration.
            previousVelocityY = currentVelocityY;

            // Affichez l'acc�l�ration en Y dans la console.
            Debug.Log("Acc�l�ration en Y : " + accelerationY);
        }
    }
}
