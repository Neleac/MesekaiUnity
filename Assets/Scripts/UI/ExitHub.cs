using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class ExitHub : MonoBehaviourPunCallbacks
{
    public void Exit()
    {
        PhotonNetwork.Disconnect();        
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("Avatar");
    }
}
