using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject avatarPrefab;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

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
        GameObject avatar = PhotonNetwork.Instantiate(avatarPrefab.name, new Vector3(65f, 22.175f, 43f), Quaternion.identity);
    }
}
