using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f; // Vitesse de d�placement du personnage
    public float turnSpeed = 10f; // Vitesse de rotation du personnage
    public float jumpForce = 7f; // Force du saut
    public Transform cameraTransform; // Transform de la cam�ra
    public LayerMask groundLayer; // Layer pour d�tecter le sol
    public float groundCheckDistance; // Distance pour v�rifier si le personnage est au sol
    [NonSerialized] public bool EnableMovement = false;
    public bool IsGrounded
    {
        get => _isGrounded;
        private set
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
                /*AnimationServerRpc(NetworkManager.Singleton.LocalClientId, ANIM.INAIR, !value);*/
            }
            _isGrounded = value;
            AnimationServerRpc(NetworkManager.Singleton.LocalClientId, ANIM.INAIR, !value);
        }
    }
    public bool JumpPressed
    {
        get => _jumpPressed;
        private set
        {
            if (value != _jumpPressed)
            {
                //AnimationServerRpc(NetworkManager.Singleton.LocalClientId, ANIM.JUMP, value);
            }
            _jumpPressed = value;
            AnimationServerRpc(NetworkManager.Singleton.LocalClientId, ANIM.JUMP, value);
        }
    }

    public bool LockCursorPressed;

    private IEnumerator leapIntoSpace = null;
    private Rigidbody rb; // Rigidbody du personnage
    private Vector3 movement; // Direction du mouvement bas�e sur les entr�es de l'utilisateur
    private bool _isGrounded;
    private bool _jumpPressed;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        print(EnableMovement);
        if (!IsOwner)
        {
            cameraTransform.gameObject.SetActive(false);
            EnableMovement = false;
        }
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        SceneManager.sceneLoaded += BlockPlayer;
        if (!cameraTransform)
        {
            Debug.LogError("Camera Transform n'est pas assign� au script CharacterControllerWithCamera.");
        }
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void BlockPlayer(Scene arg0, LoadSceneMode arg1)
    {
        if (IsOwner)
        EnableMovement = arg0.name == "Lobby" ? true : false;
    }
    public void ToggleCursorLock()
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None: CursorLockMode.Locked;
    }

    private void Update()
    {
        if (!EnableMovement) return;

        // R�cup�rer les entr�es de l'utilisateur pour le mouvement et le saut
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        JumpPressed = Input.GetButtonDown("Jump"); // D�tecter si l'utilisateur appuie sur la barre espace
        LockCursorPressed = Input.GetKey(KeyCode.L);

        if (LockCursorPressed && IsServer)
        {
            ToggleCursorLock();
        }
        
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
            RotateServerRpc(NetworkManager.Singleton.LocalClientId, movement);
        }

        // V�rifier si le personnage est au sol
        IsGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundLayer);

        if (IsGrounded && JumpPressed && rb.velocity.y <= 0.0001)
        {
            AddForceServerRpc(NetworkManager.Singleton.LocalClientId);
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

        // Lancement de l'animation de la course
        if (movement == Vector3.zero)
            AnimationServerRpc(clientId, ANIM.MOVE, false);
        else
            AnimationServerRpc(clientId, ANIM.MOVE, true);
    }
    [ServerRpc]
    private void AddForceServerRpc(ulong clientId)
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
        if (animator.GetBool(animation) != active)
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