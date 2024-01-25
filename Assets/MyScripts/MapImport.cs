using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class MapImport : MonoBehaviour
{
    public Map Map { get;set; }
    public void GetJSONMap(string pathjsonfile)
    {
        //DEBUG LINE
        if (pathjsonfile == string.Empty)
        {
            pathjsonfile = Path.Combine(Application.streamingAssetsPath, "map.json");
        }
        string jsonContent = File.ReadAllText(pathjsonfile);
        Map = JsonConvert.DeserializeObject<Map>(jsonContent);
    }
}
