using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private GameObject nameInput;
    private GameObject startButton;

    [SerializeField] private NetworkManager networkManager;
    private MessageQueue msgQueue;

	void Start()
	{
        nameInput = transform.Find("Name Input").gameObject;
        startButton = transform.Find("Start Button").gameObject;

        msgQueue = networkManager.GetComponent<MessageQueue>();
		msgQueue.AddCallback(Constants.SMSG_JOIN, OnResponseJoin);
    }

    public void OnStartClick()
    {
        string name = nameInput.GetComponent<TMP_InputField>().text;
        if (name.Length > 0)
        {
            bool connected = networkManager.SendJoinRequest(name);
            if (!connected) Debug.LogWarning("SendJoinRequest failed.");

            //SceneManager.LoadScene("Hub");
        }
	}

    public void OnResponseJoin(ExtendedEventArgs eventArgs)
    {
        // TODO: create avatar with name of player that joined
        
        ResponseJoinEventArgs args = eventArgs as ResponseJoinEventArgs;

        print("JOINED: " + args.name);
    }
}
