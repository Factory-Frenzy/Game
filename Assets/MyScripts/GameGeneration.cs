using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameGeneration : NetworkBehaviour
{
    private List<MapObjectData> Maps;
    private NetworkVariable<bool> _findSpawn = new NetworkVariable<bool>(false);
    void Start()
    {
        _findSpawn.OnValueChanged += (@previous, @next) => FindSpawn();

        if (NetworkManager.Singleton.IsServer)
        {
            MapImport mapImport = NetworkManager.Singleton.gameObject.GetComponent<MapImport>();
            Maps = mapImport.Maps;
            Destroy(mapImport);
            //GenerationMap();
            _findSpawn.Value = !_findSpawn.Value;
        }
        else if (_findSpawn.Value)
        {
            FindSpawn();
        }
    }

    private void GenerationMap()
    {
        foreach (var item in Maps)
        {
            GameObject prefab_name = Resources.Load("MyPrefabs\\" + item.name + " Variant") as GameObject;
            var obj = Instantiate(prefab_name, item.position, item.rotation);
            obj.GetComponent<NetworkObject>().Spawn();
        }
    }

    private void FindSpawn()
    {
        // Stop mouvement player
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerMovement>().DisableMovement = true;
        // Set positon player in a spawn place
        NetworkManager.Singleton.LocalClient.PlayerObject.transform.position = 
            GameObject.FindGameObjectsWithTag("Spawn")
            [NetworkManager.Singleton.LocalClientId].transform.position;
        // Set rotation player 
        NetworkManager.Singleton.LocalClient.PlayerObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        GameManager.Instance.ImReadyServerRpc();
    }
}

public class MapObjectData
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
}