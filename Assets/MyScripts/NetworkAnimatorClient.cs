using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class NetworkAnimatorClient : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}

/*private PlayerMovement _playerMovement;
private Animator _animator;
private bool _memoire = false;
public override void OnNetworkSpawn()
{
    base.OnNetworkSpawn();
    _playerMovement = this.GetComponent<PlayerMovement>();
    _animator = this.GetComponent<Animator>();
}

private void Update()
{
    if (_playerMovement.IsMoving != _memoire)
    {
        SynchroniseAnimationServerRpc(_playerMovement.IsMoving);
        _memoire = _playerMovement.IsMoving;
    }
}

[ServerRpc]
private void SynchroniseAnimationServerRpc(bool isMoving)
{
    SynchroniseAnimationClientRpc(isMoving);
}

[ClientRpc]
private void SynchroniseAnimationClientRpc(bool isMoving)
{
    _animator.SetBool("IsMoving", isMoving);
}*/
