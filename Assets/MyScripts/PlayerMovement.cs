using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode;
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
    public float groundCheckDistance = 0.2f; // Distance pour vérifier si le personnage est au sol
    private bool _isMoving;
    private bool IsMoving
    {
        get { return _isMoving; }
        set
        {
            if (value != _isMoving)
            {
                if (value)
                {
                    //animator.SetBool("IsMoving", true);
                    AnimationServerRpc(NetworkManager.Singleton.LocalClientId, "IsMoving", true);
                }
                else
                {
                    //animator.SetBool("IsMoving", false);
                    AnimationServerRpc(NetworkManager.Singleton.LocalClientId, "IsMoving", false);
                }
                _isMoving = value;
            }
        }
    }
    public bool DisableMovement
    {
        get { return _disableMovement; }
        set
        {
            if (value)
            {
                IsMoving = false;
                //animator.SetBool("Jump", false);
                AnimationServerRpc(NetworkManager.Singleton.LocalClientId, "Jump", false);
                AnimationServerRpc(NetworkManager.Singleton.LocalClientId, "IsGrounded", true);
                AnimationServerRpc(NetworkManager.Singleton.LocalClientId, "IsMoving", false);
                _disableMovement = true;
                
            }
            else
            {
                _disableMovement = false;
            }
        }
    }

    private Rigidbody rb; // Rigidbody du personnage
    private Vector3 movement; // Direction du mouvement basée sur les entrées de l'utilisateur
    private bool IsGrounded
    {
        get { return _isGrounded; }
        set
        {
            if(!_isGrounded && value)
            {
                //animator.SetBool("Jump", false);
                AnimationServerRpc(NetworkManager.Singleton.LocalClientId, "Jump", false);
            }
            Tailspin = value;
            _isGrounded = value;
        }
    }
    private bool _isGrounded;
    private Animator animator;
    private bool _disableMovement = false;
    private IEnumerator _tailspinCoroutine = null;
    private bool _tailspin;
    private bool Tailspin
    {
        get { return _tailspin; }
        set
        {
            if (Tailspin != value)
            {
                if (!value)
                {
                    _tailspinCoroutine = TailspinCoroutine();
                    StartCoroutine(_tailspinCoroutine);
                }
                else
                {
                    if(_tailspinCoroutine != null)
                    StopCoroutine(_tailspinCoroutine);
                }
                _tailspin = value;
            }
        }
    }

    [ServerRpc]
    private void GetCheckpointForThisPlayerServerRpc(ulong playerId)
    {
        Vector3 checkpoint = GameManager.Instance.GetPlayerInfo(playerId).CheckpointPosition;
        if (checkpoint == Vector3.zero)
        {
            checkpoint = GameObject.FindGameObjectsWithTag("Spawn")[playerId].transform.position;
        }
        NetworkManager.Singleton.ConnectedClients[playerId].PlayerObject.gameObject.transform.position = checkpoint;
    }

    private IEnumerator TailspinCoroutine()
    {
        yield return new WaitForSeconds(5);
        print("Saut dans le vide detecter");
        GetCheckpointForThisPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
    }

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
        rb = GetComponent<Rigidbody>(); // Récupérer le Rigidbody du personnage
        if (!cameraTransform)
        {
            Debug.LogError("Camera Transform n'est pas assigné au script CharacterControllerWithCamera.");
        }
    }

    private void Update()
    {
        if (_disableMovement) return;

        // Récupérer les entrées de l'utilisateur pour le mouvement et le saut
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        bool jumpPressed = Input.GetButtonDown("Jump"); // Détecter si l'utilisateur appuie sur la barre espace
        IsMoving = moveVertical != 0 || moveHorizontal != 0 ? true : false;

        //animator.SetBool("IsMoving", IsMoving);

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
            //rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.deltaTime);
            RotateServerRpc(NetworkManager.Singleton.LocalClientId, movement);
        }

        // Vérifier si le personnage est au sol
        IsGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundLayer);
        var IsGrounded2 = Physics.CheckSphere(transform.position, 0.35f, groundLayer);

        if (IsGrounded2 != animator.GetBool("IsGrounded"))
        {
            AnimationServerRpc(NetworkManager.Singleton.LocalClientId, "IsGrounded", IsGrounded2);
        }
        
        // Gérer le saut
        if (IsGrounded && jumpPressed)
        {
            //animator.SetBool("Jump", true);
            AnimationServerRpc(NetworkManager.Singleton.LocalClientId, "Jump", true);
            //rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            AddForceServerRpc(NetworkManager.Singleton.LocalClientId, jumpForce);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, groundCheckDistance);
    }

    private void FixedUpdate()
    {
        if (_disableMovement) return;
        if (movement == Vector3.zero) return;
        //rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
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
