using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public Transform target; // Le personnage que la caméra suit
    public float sensitivity = 5f; // Sensibilité de la rotation de la caméra
    public float minYAngle = -40f; // Angle minimum en Y pour la caméra
    public float maxYAngle = 85f; // Angle maximum en Y pour la caméra

    private Vector3 initialOffset; // Offset initial entre la caméra et le personnage
    private Quaternion initialRotation; // Rotation initiale de la caméra
    private float currentX = 0f;
    private float currentY = 0f;

    private void Start()
    {
        initialOffset = transform.position - target.position;
        initialRotation = transform.rotation;

        // Ajuster l'angle Y initial en fonction de la rotation de la caméra
        currentY = transform.eulerAngles.x;
        currentX = transform.eulerAngles.y;
    }

    private void Update()
    {
        currentX += Input.GetAxis("Mouse X") * sensitivity;
        currentY -= Input.GetAxis("Mouse Y") * sensitivity;
        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
    }

    private void LateUpdate()
    {
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0) * initialRotation;
        transform.position = target.position + rotation * initialOffset;
        transform.LookAt(target.position);
    }
}
