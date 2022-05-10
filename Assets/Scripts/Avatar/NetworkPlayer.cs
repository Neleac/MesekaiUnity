using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;
using StarterAssets;

public class NetworkPlayer : MonoBehaviourPun, IPunObservable
{   
    [SerializeField] private GameObject playerFollowCam;
    [SerializeField] private SkinnedMeshRenderer faceMesh;

    void Start()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            playerFollowCam.SetActive(false);

            //GetComponent<Animator>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
            GetComponent<PlayerInput>().enabled = false;
            GetComponent<ThirdPersonController>().enabled = false;
            GetComponent<BasicRigidBodyPush>().enabled = false;
            GetComponent<StarterAssetsInputs>().enabled = false;
            GetComponent<FaceSolver>().enabled = false;
            //GetComponent<MotionToggle>().enabled = false;
        }
    }

    void Update()
    {
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        int nBlendshapes = faceMesh.sharedMesh.blendShapeCount;
        for (int i = 0; i < nBlendshapes; i++)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(faceMesh.GetBlendShapeWeight(i));
            }
            else
            {
                faceMesh.SetBlendShapeWeight(i, (float)stream.ReceiveNext());
            }
        }
    }
}
