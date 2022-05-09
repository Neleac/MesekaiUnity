using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject nameInput;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void OnStartClick()
    {
        string playerName = nameInput.GetComponent<TMP_InputField>().text;
        if (playerName.Length > 0 && !PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.LogWarning("Please enter name.");
        }
	}

	public override void OnConnectedToMaster()
	{
		RoomOptions roomOptions = new RoomOptions();
		PhotonNetwork.JoinOrCreateRoom("Mesekai", roomOptions, null);
        PhotonNetwork.NickName = nameInput.GetComponent<TMP_InputField>().text;
	}

    public override void OnJoinedRoom() 
    {
        SceneManager.LoadScene("Hub");
    }
}
