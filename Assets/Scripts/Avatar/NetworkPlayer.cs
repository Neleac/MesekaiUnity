using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;
using StarterAssets;

public class NetworkPlayer : MonoBehaviourPun
{   
    [SerializeField] private GameObject playerFollowCam;

    void Start()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            playerFollowCam.SetActive(false);

            GetComponent<CharacterController>().enabled = false;
            GetComponent<PlayerInput>().enabled = false;
            GetComponent<ThirdPersonController>().enabled = false;
            GetComponent<BasicRigidBodyPush>().enabled = false;
            GetComponent<StarterAssetsInputs>().enabled = false;
            GetComponent<PoseSolver>().enabled = false;
            GetComponent<HandSolver>().enabled = false;
            GetComponent<FaceSolver>().enabled = false;
            GetComponent<MotionToggle>().enabled = false;
        }
    }

    void Update()
    {
    }
}
