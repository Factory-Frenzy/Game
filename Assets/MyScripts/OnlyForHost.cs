using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnlyForHost : MonoBehaviour
{
    void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            this.gameObject.SetActive(true);
        } 
    }
}
