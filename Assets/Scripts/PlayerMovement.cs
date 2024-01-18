using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Vitesse de déplacement du personnage
    public float turnSpeed = 10f; // Vitesse de rotation du personnage
    public float jumpForce = 7f; // Force du saut
    public Transform cameraTransform; // Transform de la caméra
    public LayerMask groundLayer; // Layer pour détecter le sol
    public float groundCheckDistance = 0.2f; // Distance pour vérifier si le personnage est au sol

    private Rigidbody rb; // Rigidbody du personnage
    private Vector3 movement; // Direction du mouvement basée sur les entrées de l'utilisateur
    private bool isGrounded; // Est-ce que le personnage est au sol

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>(); // Récupérer le Rigidbody du personnage
        if (!cameraTransform)
        {
            Debug.LogError("Camera Transform n'est pas assigné au script CharacterControllerWithCamera.");
        }
    }

    private void Update()
    {
        // Récupérer les entrées de l'utilisateur pour le mouvement et le saut
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        bool jumpPressed = Input.GetButtonDown("Jump"); // Détecter si l'utilisateur appuie sur la barre espace

        animator.SetBool("IsMoving", moveVertical != 0 || moveHorizontal != 0 ? true : false);

        // Calculer la direction du mouvement
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        movement = (forward * moveVertical + right * moveHorizontal).normalized;

        // Mettre à jour la rotation du personnage
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // Vérifier si le personnage est au sol
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundLayer);

        // Gérer le saut
        if (isGrounded && jumpPressed)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        // Appliquer le mouvement au Rigidbody
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public void OnFootstep() { }
}
