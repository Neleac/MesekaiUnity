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

    void Awake()
    {
        networkManager = GameObject.Find("Network Manager").GetComponent<NetworkManager>();;
        
        msgQueue = networkManager.GetComponent<MessageQueue>();
        msgQueue.AddCallback(Constants.SMSG_MOVE, OnResponseMove);

        // set client avatar name
        avatar.name = networkManager.playerName + " Avatar";

        // instantiate avatar for players joined BEFORE client
        foreach (string otherName in networkManager.otherPlayers)
        {
            GameObject newAvatar = Instantiate(avatar);
            newAvatar.name = otherName + " Avatar";
            
            // disable client control components
            newAvatar.GetComponent<Animator>().enabled = false;
            newAvatar.GetComponent<CharacterController>().enabled = false;
            newAvatar.GetComponent<PlayerInput>().enabled = false;
            newAvatar.GetComponent<ThirdPersonController>().enabled = false;
            newAvatar.GetComponent<BasicRigidBodyPush>().enabled = false;
            newAvatar.GetComponent<StarterAssetsInputs>().enabled = false;
            newAvatar.GetComponent<PoseSolver>().enabled = false;
            newAvatar.GetComponent<HandSolver>().enabled = false;
            newAvatar.GetComponent<FaceSolver>().enabled = false;
            newAvatar.GetComponent<MotionToggle>().enabled = false;
            newAvatar.GetComponent<NetworkPlayer>().enabled = false;
        }
    }

    void Update()
    {
        
    }

    // called every frame from every other player
    public void OnResponseMove(ExtendedEventArgs eventArgs)
    {
        ResponseMoveEventArgs args = eventArgs as ResponseMoveEventArgs;

        Transform avatarTf = GameObject.Find(args.playerName + " Avatar").transform;
        avatarTf.position = args.position;
        avatarTf.eulerAngles = args.rotation;

        Transform spine = avatarTf.Find("Armature/Hips/Spine");
        int finalIdx = setJointAngles(spine, args.jointAngles, 0);
        Debug.Assert(finalIdx == args.jointAngles.Length);
    }

    private int setJointAngles(Transform jointTf, float[] angles, int idx)
    {
        jointTf.eulerAngles = new Vector3(angles[idx++], angles[idx++], angles[idx++]);

		// DFS
		foreach (Transform child in jointTf)
        {
            idx = setJointAngles(child, angles, idx);
        }

        return idx;
    }
}
