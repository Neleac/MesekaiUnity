using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    private NetworkManager networkManager;
    private MessageQueue msgQueue;

    void Start()
    {
        networkManager = GameObject.Find("Network Manager").GetComponent<NetworkManager>();;
        msgQueue = networkManager.GetComponent<MessageQueue>();
        msgQueue.AddCallback(Constants.SMSG_MOVE, OnResponseMove);
    }

    void Update()
    {
        bool connected = networkManager.SendMoveRequest(networkManager.playerName, transform.position);
        if (!connected) Debug.LogWarning("SendMoveRequest failed.");
    }

    public void OnResponseMove(ExtendedEventArgs eventArgs)
    {
        ResponseMoveEventArgs args = eventArgs as ResponseMoveEventArgs;

        if (gameObject.name.Split(" ")[0].Equals(args.playerName))
        {
            transform.position = args.transform;
        }
    }
}
