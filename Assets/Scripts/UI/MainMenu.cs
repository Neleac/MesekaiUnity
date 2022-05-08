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

    private Vector3 startPos;
    private Quaternion startRot;

	void Start()
	{
        networkManager = GameObject.Find("Network Manager").GetComponent<NetworkManager>();;
        msgQueue = networkManager.GetComponent<MessageQueue>();
        msgQueue.AddCallback(Constants.SMSG_JOIN, OnResponseJoin);

        nameInput = transform.Find("Name Input").gameObject;
        startButton = transform.Find("Start Button").gameObject;

        // TODO: randomize new player spawn
        startPos = new Vector3(65f, 22.175f, 43f);
        startRot = Quaternion.Euler(0, 0, 0);
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
                GameObject avatar = GameObject.Find("Player Manager").GetComponent<PlayerManager>().avatar;
                GameObject otherAvatar = Instantiate(avatar, startPos, startRot);
                otherAvatar.name = args.playerName + " Avatar";
                
                // disable client control components
                otherAvatar.transform.Find("PlayerFollowCamera").gameObject.SetActive(false);
                //otherAvatar.GetComponent<Animator>().enabled = false;
                otherAvatar.GetComponent<CharacterController>().enabled = false;
                otherAvatar.GetComponent<PlayerInput>().enabled = false;
                //otherAvatar.GetComponent<ThirdPersonController>().enabled = false;
                otherAvatar.GetComponent<BasicRigidBodyPush>().enabled = false;
                otherAvatar.GetComponent<StarterAssetsInputs>().enabled = false;
                otherAvatar.GetComponent<PoseSolver>().enabled = false;
                otherAvatar.GetComponent<HandSolver>().enabled = false;
                otherAvatar.GetComponent<FaceSolver>().enabled = false;
                otherAvatar.GetComponent<MotionToggle>().enabled = false;
                otherAvatar.GetComponent<NetworkPlayer>().enabled = false;

                networkManager.otherJoined = true;
            }
        }
    }
}
