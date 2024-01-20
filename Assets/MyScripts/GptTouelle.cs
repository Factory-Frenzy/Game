using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GptTouelle : NetworkBehaviour
{
    public Transform OutputCanon;
    public GameObject Boulet;
    public float SpeedRotation = 5f;
    public float BouletForceForward = 10f;
    public float BouletForceUp = 2f;
    public float AngleShoot = 30f;
    private bool _enableShoot = true;
    private GameObject Intru = null;

    private void Update()
    {
        if (Intru && IsServer)
        {
            var lookPos = Intru.transform.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * SpeedRotation);
            if (Vector3.Angle(Intru.transform.position - OutputCanon.position, transform.forward) <= AngleShoot)
            {
                if (_enableShoot)
                {
                    _enableShoot = false;
                    StartCoroutine(Shoot());
                }
            }
        }
    }

    private IEnumerator Shoot()
    {
        var boulet = Instantiate(Boulet, OutputCanon.position, Quaternion.identity);
        boulet.GetComponent<NetworkObject>().Spawn();
        boulet.GetComponent<Rigidbody>().AddForce(transform.forward * BouletForceForward + Vector3.up * BouletForceUp, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f);
        _enableShoot = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && IsServer)
        {
            Debug.Log("enter");
            Intru = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && IsServer)
        {
            Debug.Log("exit");
            Intru = null;
        }
    }
}