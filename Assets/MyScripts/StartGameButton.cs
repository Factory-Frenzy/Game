using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void StartGameButton()
    {
        GameManager.Instance.InitClientsInfos();
        foreach (var item in NetworkManager.Singleton.ConnectedClientsList)
        {
            item.PlayerObject.Despawn();
        }
        NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
