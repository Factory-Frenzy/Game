using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class MapImport : MonoBehaviour
{
    public Map Map { get; private set; }
    public void GetJSONMap(string pathjsonfile)
    {
        string jsonContent;
        if (pathjsonfile == string.Empty)
        {
            jsonContent = Resources.Load("maps/map").ToString();
        }
        else
        {
            jsonContent = File.ReadAllText(pathjsonfile);
        }
        Map = JsonConvert.DeserializeObject<Map>(jsonContent);
    }
}
