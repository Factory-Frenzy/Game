using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class VentilloScript : NetworkBehaviour
{
    private float VentilloForce = 5f;
    private Vector3 VentDirection;
    private List<Rigidbody> playersRb = new List<Rigidbody>();

    private void Start()
    {
        VentDirection = transform.forward;
    }
    private void FixedUpdate()
    {
        foreach (var playerRb in playersRb)
        {
            playerRb.MovePosition(playerRb.position + VentilloForce * VentDirection * Time.fixedDeltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            if (other.GetComponent<CapsuleCollider>().GetType() == typeof(CapsuleCollider) &&
                other.gameObject.CompareTag("Player"))
            {
                print("Force ventillo Start");
                playersRb.Add(other.gameObject.GetComponent<Rigidbody>());
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (IsServer)
        {
            if (other.GetComponent<CapsuleCollider>().GetType() == typeof(CapsuleCollider) &&
                other.gameObject.CompareTag("Player"))
            {
                print("Force ventillo Stop");
                playersRb.Remove(other.gameObject.GetComponent<Rigidbody>());
            }
        }
    }
}
