using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnlyForHost : MonoBehaviour
{
    void Start()
    {
/*        print("IsServer: "+ NetworkManager.Singleton.IsServer);
        print("IsHost: "+ NetworkManager.Singleton.IsHost);
        print("IsClient: "+ NetworkManager.Singleton.IsClient);*/
        if (!NetworkManager.Singleton.IsServer)
        {
            this.gameObject.SetActive(false);
        } 
    }
}
