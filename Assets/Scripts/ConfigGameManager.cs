using AnotherFileBrowser.Windows;
using System.IO;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ConfigGameManager : MonoBehaviour
{
    [SerializeField] private GameObject HostClientMenu;
    [SerializeField] private GameObject HostMenu;
    [SerializeField] private GameObject ClientMenu;
    [SerializeField] private TMP_InputField IpServerInput;
    [SerializeField] private TextMeshProUGUI SceneNameText;

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
        switch(isHost)
        {
            case true:
                // if json est good
                var result = NetworkManager.Singleton.StartHost();
                if (result)
                {
                    NetworkManager.Singleton.SceneManager.LoadScene("Lobby",
                        UnityEngine.SceneManagement.LoadSceneMode.Single);
                }
                break;
            case false:
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
                break;
        }
    }
    public void OpenFileBrowser()
    {
        var bp = new BrowserProperties();
        bp.filter = "JSON files (*.json) | *.json";
        bp.filterIndex = 0;

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            SceneNameText.text = path.Split("\\")[path.Split("\\").Length - 1];
            string jsonContent = File.ReadAllText(path);
        });
    }
}
public enum MenuName
{
    HOST = 1,
    CLIENT = 2,
    HOST_CLIENT = 3
}
