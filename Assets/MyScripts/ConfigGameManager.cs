#if UNITY_STANDALONE_WIN
using AnotherFileBrowser.Windows;
#endif
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfigGameManager : MonoBehaviour
{
    [SerializeField] private GameObject HostClientMenu;
    [SerializeField] private GameObject HostMenu;
    [SerializeField] private GameObject ClientMenu;
    [SerializeField] private TMP_InputField IpServerInput;
    [SerializeField] private TextMeshProUGUI SceneNameText;
    private string _pathjsonfile = string.Empty;

    public void NextMenu(int menu)
    {
        switch (menu)
        {
            case (int)MenuName.CLIENT:
                HostClientMenu.SetActive(false);
                ClientMenu.SetActive(true);
                break;
            case (int)MenuName.HOST:
                HostClientMenu.SetActive(false);
                HostMenu.SetActive(true);
                break;
            case (int)MenuName.HOST_CLIENT:
                break;
            default:
                break;
        }
    }
    public void StartGame(bool isHost)
    {
        if (isHost)
        {
            //if (_pathjsonfile == string.Empty) return;

            var scene = SceneManager.LoadSceneAsync("Lobby");
            scene.completed += (AsyncOperation operation) =>
            {
                MapImport mapImport = NetworkManager.Singleton.gameObject.AddComponent<MapImport>();
                mapImport.GetJSONMap(_pathjsonfile);
                if (NetworkManager.Singleton.StartHost())
                    NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
            };
        }
        else
        {
            var utp = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            // if addresse est good
            if (IpServerInput.text != string.Empty)
            {
                utp.SetConnectionData(IpServerInput.text, 7777);
            }
            else
            {
                utp.SetConnectionData("127.0.0.1", 7777);
            }
            NetworkManager.Singleton.StartClient();
        }
    }

    public void OpenFileBrowser()
    {
#if UNITY_STANDALONE_WIN
        var bp = new BrowserProperties();
        bp.filter = "JSON files (*.json) | *.json";
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            _pathjsonfile = path;
            SceneNameText.text = path.Split("\\")[path.Split("\\").Length - 1];
        });
#endif
    }
}
public enum MenuName
{
    HOST = 1,
    CLIENT = 2,
    HOST_CLIENT = 3
}
