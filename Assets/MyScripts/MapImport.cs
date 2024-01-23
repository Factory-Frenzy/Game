using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapImport : MonoBehaviour
{
    public Map Map { get;set; }
    public void GetJSONMap(string pathjsonfile)
    {
        //DEBUG LINE
        if (pathjsonfile == string.Empty) pathjsonfile = "C:\\Users\\arnau\\Documents\\Annee 5\\Projet RA RV\\Projet_final\\Multi\\Assets\\Resources\\Maps\\map.json";
        string jsonContent = File.ReadAllText(pathjsonfile);
        Map = JsonConvert.DeserializeObject<Map>(jsonContent);
    }
}
