using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private NetworkManager networkManager;
    private MessageQueue msgQueue;

    private GameObject nameInput;
    private GameObject startButton;

	void Start()
	{
        networkManager = GameObject.Find("Network Manager").GetComponent<NetworkManager>();;
        msgQueue = networkManager.GetComponent<MessageQueue>();

        nameInput = transform.Find("Name Input").gameObject;
        startButton = transform.Find("Start Button").gameObject;
    }

    public void OnStartClick()
    {
        string name = nameInput.GetComponent<TMP_InputField>().text;
        if (name.Length > 0)
        {
            bool connected = networkManager.SendJoinRequest(name);
            if (!connected) Debug.LogWarning("SendJoinRequest failed.");

            SceneManager.LoadScene("Hub");
        }
	}
}
