using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class NetworkPlayer : MonoBehaviour
{
    private NetworkManager networkManager;

    private StarterAssetsInputs input;
    private ThirdPersonController controller;

    void Start()
    {
        networkManager = GameObject.Find("Network Manager").GetComponent<NetworkManager>();

        input = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();
    }

    void Update()
    {
        // TODO: send animator state while moving, send transforms when idle
        if (input.move.magnitude > 0 || !controller.Grounded || networkManager.otherJoined)
        {
            bool connected = networkManager.SendMoveRequest(networkManager.playerName, transform);
            if (!connected) Debug.LogWarning("SendMoveRequest failed.");

            networkManager.otherJoined = false;
        }
    }
}
