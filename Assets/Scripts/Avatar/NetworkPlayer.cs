using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class NetworkPlayer : MonoBehaviour
{
    private NetworkManager networkManager;

    void Start()
    {
        networkManager = GameObject.Find("Network Manager").GetComponent<NetworkManager>();
    }

    void Update()
    {
        bool connected = networkManager.SendMoveRequest(networkManager.playerName, transform);
        if (!connected) Debug.LogWarning("SendMoveRequest failed.");
    }
}
