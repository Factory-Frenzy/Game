using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    [NonSerialized]
    public GameManager Instance = null;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartGame()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var scene = SceneManager.LoadSceneAsync("Game");
            scene.completed += (AsyncOperation operation) =>
            {
                if (NetworkManager.Singleton.StartHost())
                    NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
            };
        }
    }
}
