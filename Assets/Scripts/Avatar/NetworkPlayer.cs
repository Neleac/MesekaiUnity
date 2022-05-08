using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class NetworkPlayer : MonoBehaviour
{
    private NetworkManager networkManager;
    private Animator animator;

    void Start()
    {
        networkManager = GameObject.Find("Network Manager").GetComponent<NetworkManager>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator.enabled || networkManager.otherJoined)
        {
            // WASD movement, or other player initial sync
            bool connected = networkManager.SendMoveRequest(networkManager.playerName, transform, false);
            if (!connected) Debug.LogWarning("SendMoveRequest failed.");

            networkManager.otherJoined = false;
        }
        else if (GetComponent<MotionToggle>().enabled)
        {
            // motion tracking
            bool connected = networkManager.SendMoveRequest(networkManager.playerName, transform, true);
            if (!connected) Debug.LogWarning("SendMoveRequest failed.");
        }
    }
}
