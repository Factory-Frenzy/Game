using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f; // Vitesse de d�placement du personnage
    public float turnSpeed = 10f; // Vitesse de rotation du personnage
    public float jumpForce = 7f; // Force du saut
    public Transform cameraTransform; // Transform de la cam�ra
    public LayerMask groundLayer; // Layer pour d�tecter le sol
    public float groundCheckDistance = 0.2f; // Distance pour v�rifier si le personnage est au sol
    public bool IsMoving { get; set; }
    public bool DisableMovement
    {
        get { return _disableMovement; }
        set
        {
            if (value)
            {
                IsMoving = false;
                _disableMovement = true;
            }
            else
            {
                _disableMovement = false;
            }
        }
    }

    private Rigidbody rb; // Rigidbody du personnage
    private Vector3 movement; // Direction du mouvement bas�e sur les entr�es de l'utilisateur
    private bool IsGrounded
    {
        get { return _isGrounded; }
        set
        {
            if(!_isGrounded && value)
            {
                animator.SetBool("Jump", false);
            }
            _isGrounded = value;
        }
    }
    private bool _isGrounded;
    private Animator animator;
    private bool _disableMovement = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
        {
            cameraTransform.gameObject.SetActive(false);
            _disableMovement = true;
            //Destroy(this);
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>(); // R�cup�rer le Rigidbody du personnage
        if (!cameraTransform)
        {
            Debug.LogError("Camera Transform n'est pas assign� au script CharacterControllerWithCamera.");
        }
    }

    private void Update()
    {
        if (_disableMovement) return;

        // R�cup�rer les entr�es de l'utilisateur pour le mouvement et le saut
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        bool jumpPressed = Input.GetButtonDown("Jump"); // D�tecter si l'utilisateur appuie sur la barre espace
        IsMoving = moveVertical != 0 || moveHorizontal != 0 ? true : false;

        animator.SetBool("IsMoving", IsMoving);

        // Calculer la direction du mouvement
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        movement = (forward * moveVertical + right * moveHorizontal).normalized;

        // Mettre � jour la rotation du personnage
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // V�rifier si le personnage est au sol
        IsGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundLayer);

        // G�rer le saut
        if (IsGrounded && jumpPressed)
        {
            animator.SetBool("Jump", true);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        if (_disableMovement) return;
        // Appliquer le mouvement au Rigidbody
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public void OnFootstep() 
    {
        //print("OnFootstep");
    }

    public void OnLand() { }
}
