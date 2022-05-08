using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;
using Mediapipe.Unity.Holistic;

public class PlayerManager : MonoBehaviour
{
    private NetworkManager networkManager;
    private MessageQueue msgQueue;

    [SerializeField] public GameObject avatar;
    [SerializeField] private GameObject mediapipeSolution;

    void Awake()
    {
        networkManager = GameObject.Find("Network Manager").GetComponent<NetworkManager>();;
        
        msgQueue = networkManager.GetComponent<MessageQueue>();
        msgQueue.AddCallback(Constants.SMSG_MOVE, OnResponseMove);
        msgQueue.AddCallback(Constants.SMSG_ANIMATE, OnResponseAnimate);

        // instantiate avatar for this client
        GameObject myAvatar = Instantiate(avatar);
        myAvatar.name = networkManager.playerName + " Avatar";
        mediapipeSolution.GetComponent<HolisticTrackingSolution>().faceSolver = myAvatar.GetComponent<FaceSolver>();
        mediapipeSolution.GetComponent<HolisticTrackingSolution>().poseSolver = myAvatar.GetComponent<PoseSolver>();
        mediapipeSolution.GetComponent<HolisticTrackingSolution>().handSolver = myAvatar.GetComponent<HandSolver>();

        // instantiate avatar for players joined BEFORE client
        foreach (string otherName in networkManager.otherPlayers)
        {
            GameObject otherAvatar = Instantiate(avatar);
            otherAvatar.name = otherName + " Avatar";
            
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
        }
    }

    void Update()
    {
        
    }

    // called every frame from every other player
    public void OnResponseMove(ExtendedEventArgs eventArgs)
    {
        ResponseMoveEventArgs args = eventArgs as ResponseMoveEventArgs;

        GameObject avatar = GameObject.Find(args.playerName + " Avatar");
        Transform avatarTf = avatar.transform;
        avatarTf.position = args.position;
        avatarTf.eulerAngles = args.rotation;

        if (args.jointAngles != null)
        {
            avatar.GetComponent<Animator>().enabled = false;
            avatar.GetComponent<ThirdPersonController>().enabled = false;
            
            Transform spine = avatarTf.Find("Armature/Hips/Spine");
            int finalIdx = setJointAngles(spine, args.jointAngles, 0);
            Debug.Assert(finalIdx == args.jointAngles.Length);
        }
        else
        {
            avatar.GetComponent<Animator>().enabled = true;
            avatar.GetComponent<ThirdPersonController>().enabled = true;
        }
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

    private void OnResponseAnimate(ExtendedEventArgs eventArgs)
    {
        ResponseAnimateEventArgs args = eventArgs as ResponseAnimateEventArgs;

        GameObject.Find(args.playerName + " Avatar").GetComponent<ThirdPersonController>().setAnimParams(args.animStateDict, args.animValDict);
    }
}
