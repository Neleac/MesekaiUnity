using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SerializationRate = 60;
    }

    public void OnStartClick()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
	}

	public override void OnConnectedToMaster()
	{
		RoomOptions roomOptions = new RoomOptions();
		PhotonNetwork.JoinOrCreateRoom("Mesekai", roomOptions, null);
	}

    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene("Hub");
    }
}
