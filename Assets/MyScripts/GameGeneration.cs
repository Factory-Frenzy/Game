using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameGeneration : NetworkBehaviour
{
    private List<MapObjectData> Maps;
    private List<GameObject> Spawns;
    void Start()
    {
        if (IsServer)
        {
            MapImport mapImport = NetworkManager.Singleton.gameObject.GetComponent<MapImport>();
            Maps = mapImport.Map.objectData;
            Destroy(mapImport);
            GenerationMap();
            SpawnPlayers();
            StartCoroutine(GameManager.Instance.RunGameStartCountdown());
        }
    }

    private void GenerationMap()
    {
        foreach (var item in Maps)
        {
            GameObject prefab_name = Resources.Load("MyPrefabs\\" + item.prefabName + " Variant") as GameObject;
            var obj = Instantiate(prefab_name, item.position, item.rotation);
            if (item.prefabName == "Platform Move 520")
            {
                obj.transform.Find("EndpointA").transform.position = item.endpoints.a;
                obj.transform.Find("EndpointB").transform.position = item.endpoints.b;
                obj.transform.Find("PlatformRoot").GetComponent<PlatformMovementOnline>().Speed = item.speed ?? 1f;
            }
            obj.GetComponent<NetworkObject>().Spawn();
        }
    }

    private void SpawnPlayers()
    {
        Spawns = GameObject.FindGameObjectsWithTag("Spawn").ToList();
        for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
        {
            NetworkManager.Singleton.ConnectedClients[(ulong)i].PlayerObject.GetComponent<PlayerMovement>().EnableMovement = false;
            NetworkManager.Singleton.ConnectedClients[(ulong)i].PlayerObject.transform.position = Spawns[i].transform.position;
            NetworkManager.Singleton.ConnectedClients[(ulong)i].PlayerObject.transform.rotation = Spawns[i].transform.rotation;
            GameManager.Instance.GetPlayerInfo((ulong)i).CheckpointPosition = Spawns[i].transform.position;
        }
    }
}

[Serializable]
public class MapObjectData
{
    public string prefabName;
    public ObjectType type;
    public bool dynamic;
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public float? speed;
    public Vector3 position;
    public Quaternion rotation;
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public ObjectEndpoints endpoints;
}

[SerializeField]
public class ObjectEndpoints
{
    public Vector3 a;
    public Vector3 b;
}

[Serializable]
public class Map
{
    public string name;
    public List<MapObjectData> objectData;
}

public enum ObjectType
{
    Undefined = 0,
    Platform = 1,
    Trap = 2,
}