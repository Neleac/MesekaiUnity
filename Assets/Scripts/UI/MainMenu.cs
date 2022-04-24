using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;
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
        msgQueue.AddCallback(Constants.SMSG_JOIN, OnResponseJoin);

        nameInput = transform.Find("Name Input").gameObject;
        startButton = transform.Find("Start Button").gameObject;
    }

    public void OnStartClick()
    {
        networkManager.playerName = nameInput.GetComponent<TMP_InputField>().text;
        if (networkManager.playerName.Length > 0)
        {
            bool connected = networkManager.SendJoinRequest(networkManager.playerName);
            if (!connected) Debug.LogWarning("SendJoinRequest failed.");
        }
        else
        {
            Debug.LogWarning("Please enter name.");
        }
	}

    // sent/received by this client only
    public void OnResponseJoin(ExtendedEventArgs eventArgs)
    {       
        ResponseJoinEventArgs args = eventArgs as ResponseJoinEventArgs;
        
        if (networkManager.playerName.Equals(args.playerName))
        {
            // in Main Menu, response is from self
            if (args.success) 
            {
                networkManager.otherPlayers = args.otherPlayers;
                SceneManager.LoadScene("Hub");
            }
            else 
            {
                Debug.LogWarning("Name already taken, please choose another name.");
            }
        }
        else
        {
            // in Hub, responses are from others
            if (args.success)
            {
                // instantiate avatar for players joined AFTER client
                GameObject avatar = GameObject.Find(networkManager.playerName + " Avatar");
                GameObject newAvatar = Instantiate(avatar);
                newAvatar.name = args.playerName + " Avatar";
                
                // disable client control components
                newAvatar.GetComponent<CharacterController>().enabled = false;
                newAvatar.GetComponent<PlayerInput>().enabled = false;
                newAvatar.GetComponent<ThirdPersonController>().enabled = false;
                newAvatar.GetComponent<BasicRigidBodyPush>().enabled = false;
                newAvatar.GetComponent<StarterAssetsInputs>().enabled = false;
                newAvatar.GetComponent<PoseSolver>().enabled = false;
                newAvatar.GetComponent<HandSolver>().enabled = false;
                newAvatar.GetComponent<FaceSolver>().enabled = false;
                newAvatar.GetComponent<MotionToggle>().enabled = false;
            }
        }
    }
}
