using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.InputSystem;
using StarterAssets;

public class PlayerManager : MonoBehaviour
{
    private NetworkManager networkManager;
    private MessageQueue msgQueue;

    [SerializeField] GameObject avatar;

    void Start()
    {
        networkManager = GameObject.Find("Network Manager").GetComponent<NetworkManager>();;
        msgQueue = networkManager.GetComponent<MessageQueue>();

		msgQueue.AddCallback(Constants.SMSG_JOIN, OnResponseJoin);
    }

    void Update()
    {
        
    }

    // called when another player joins
    public void OnResponseJoin(ExtendedEventArgs eventArgs)
    {
        ResponseJoinEventArgs args = eventArgs as ResponseJoinEventArgs;
        print("JOINED: " + args.name);

        GameObject newAvatar = Instantiate(avatar, avatar.transform.position, avatar.transform.rotation);
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
