using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f; // Vitesse de d�placement du personnage
    public float turnSpeed = 10f; // Vitesse de rotation du personnage
    public float jumpForce = 7f; // Force du saut
    public Transform cameraTransform; // Transform de la cam�ra
    public LayerMask groundLayer; // Layer pour d�tecter le sol
    public float groundCheckDistance = 0.2f; // Distance pour v�rifier si le personnage est au sol
    public bool EnableMovement = true;

    private Rigidbody rb; // Rigidbody du personnage
    private Vector3 movement; // Direction du mouvement bas�e sur les entr�es de l'utilisateur
    private bool isGrounded;
    private Animator animator;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
        {
            cameraTransform.gameObject.SetActive(false);
            EnableMovement = false;
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
        if (!EnableMovement) return;

        // R�cup�rer les entr�es de l'utilisateur pour le mouvement et le saut
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        bool jumpPressed = Input.GetButtonDown("Jump"); // D�tecter si l'utilisateur appuie sur la barre espace

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
            RotateServerRpc(NetworkManager.Singleton.LocalClientId, movement);
        }

        // V�rifier si le personnage est au sol
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundLayer);
        
        // G�rer le saut
        if (isGrounded && jumpPressed)
        {
            AddForceServerRpc(NetworkManager.Singleton.LocalClientId, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        if (!EnableMovement) return;

        MovePositionServerRpc(NetworkManager.Singleton.LocalClientId, movement);
    }

    [ServerRpc]
    private void MovePositionServerRpc(ulong clientId,Vector3 movement)
    {
        var rb = NetworkManager.Singleton.ConnectedClients[clientId]
            .PlayerObject.gameObject
            .GetComponent<Rigidbody>();
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
    [ServerRpc]
    private void AddForceServerRpc(ulong clientId, float jumpForce)
    {
        var rb = NetworkManager.Singleton.ConnectedClients[clientId]
            .PlayerObject.gameObject
            .GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    [ServerRpc]
    public void AnimationServerRpc(ulong clientId,string animation,bool active)
    {
        var animator = NetworkManager.Singleton.ConnectedClients[clientId]
            .PlayerObject.gameObject
            .GetComponent<Animator>();
        animator.SetBool(animation, active);
    }
    [ServerRpc]
    private void RotateServerRpc(ulong clientId,Vector3 movement)
    {
        Quaternion targetRotation = Quaternion.LookRotation(movement);
        var rb = NetworkManager.Singleton.ConnectedClients[clientId]
            .PlayerObject.gameObject
            .GetComponent<Rigidbody>();
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }


    public void OnFootstep(){}
    public void OnLand(){}
}
