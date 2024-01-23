using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f; // Vitesse de déplacement du personnage
    public float turnSpeed = 10f; // Vitesse de rotation du personnage
    public float jumpForce = 7f; // Force du saut
    public Transform cameraTransform; // Transform de la caméra
    public LayerMask groundLayer; // Layer pour détecter le sol
    public float groundCheckDistance; // Distance pour vérifier si le personnage est au sol
    [NonSerialized] public bool EnableMovement = true;
    public bool IsGrounded
    {
        get => _isGrounded;
        set
        {
            if (value != _isGrounded)
            {
                if (value)
                {
                    if (leapIntoSpace != null)
                    StopCoroutine(leapIntoSpace);
                }
                else
                {
                    leapIntoSpace = LeapIntoSpace();
                    StartCoroutine(leapIntoSpace);
                }
            }
            _isGrounded = value;
        }
    }

    private IEnumerator leapIntoSpace = null;
    private Rigidbody rb; // Rigidbody du personnage
    private Vector3 movement; // Direction du mouvement basée sur les entrées de l'utilisateur
    private bool _isGrounded;
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
        rb = GetComponent<Rigidbody>(); // Récupérer le Rigidbody du personnage
        if (!cameraTransform)
        {
            Debug.LogError("Camera Transform n'est pas assigné au script CharacterControllerWithCamera.");
        }
    }

    private void Update()
    {
        if (!EnableMovement) return;

        // Récupérer les entrées de l'utilisateur pour le mouvement et le saut
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        bool jumpPressed = Input.GetButtonDown("Jump"); // Détecter si l'utilisateur appuie sur la barre espace

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
            RotateServerRpc(NetworkManager.Singleton.LocalClientId, movement);
        }

        // Vérifier si le personnage est au sol
        IsGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundLayer);
        print(IsGrounded);
        // Gérer le saut
        if (IsGrounded && jumpPressed)
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
    [ServerRpc]
    private void GoToPreviousCheckpointServerRpc(ulong clientId)
    {
        NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject.transform.position =
            GameManager.Instance.GetPlayerInfo(clientId).CheckpointPosition;
    }

    private IEnumerator LeapIntoSpace()
    {
        if (!EnableMovement) yield return 0;
        yield return new WaitForSeconds(5);
        GoToPreviousCheckpointServerRpc(NetworkManager.Singleton.LocalClientId);
    }
    public void OnFootstep(){}
    public void OnLand(){}
}
