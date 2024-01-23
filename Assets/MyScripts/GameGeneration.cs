using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameGeneration : NetworkBehaviour
{
    private List<MapObjectData> Maps;
    private List<GameObject> Spawns;
    void Start()
    {

        if (IsServer)
        {
            MapImport mapImport = NetworkManager.Singleton.gameObject.GetComponent<MapImport>();
            Maps = mapImport.Maps;
            Destroy(mapImport);
            //GenerationMap();
            
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

    private void SpawnPlayers()
    {
        Spawns = GameObject.FindGameObjectsWithTag("Spawn").ToList();
        for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
        {
            NetworkManager.Singleton.ConnectedClients[(ulong)i].PlayerObject.GetComponent<PlayerMovement>().EnableMovement = false;
            NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform = new Vector3
        }
    }
}

public class MapObjectData
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
}