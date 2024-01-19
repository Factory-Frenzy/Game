using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;    
using UnityEngine;

public class GameGeneration : MonoBehaviour
{
    void Start()
    {
/*        GameObject prefab_name = Resources.Load("MyPrefabs\\" + item.name + " Variant") as GameObject;
        Instantiate(prefab_name, item.position, item.rotation);*/
    }
}

public class MapObjectData
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
}