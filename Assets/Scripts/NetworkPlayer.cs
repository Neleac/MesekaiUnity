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
    [SerializeField] private Transform lArmTf, rArmTf, headTf;

    [HideInInspector] public Quaternion headRot;

    void Start()
    {
        if (PhotonNetwork.IsConnected && !photonView.IsMine)
        {
            playerFollowCam.SetActive(false);

            //GetComponent<Animator>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
            GetComponent<PlayerInput>().enabled = false;
            GetComponent<ThirdPersonController>().enabled = false;
            GetComponent<BasicRigidBodyPush>().enabled = false;
            GetComponent<StarterAssetsInputs>().enabled = false;
        }

        headRot = headTf.localRotation;
    }

    void LateUpdate()
    {
        if (!photonView.IsMine) headTf.localRotation = headRot;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // facial expression
        int nBlendshapes = faceMesh.sharedMesh.blendShapeCount;
        for (int i = 0; i < nBlendshapes; i++)
        {
            if (stream.IsWriting) stream.SendNext(faceMesh.GetBlendShapeWeight(i));
            else faceMesh.SetBlendShapeWeight(i, (float)stream.ReceiveNext());
        }

        // head turn
        if (stream.IsWriting)
        {
            stream.SendNext(headRot.x);
            stream.SendNext(headRot.y);
            stream.SendNext(headRot.z);
            stream.SendNext(headRot.w);
        }
        else
        {
            float x = (float)stream.ReceiveNext();
            float y = (float)stream.ReceiveNext();
            float z = (float)stream.ReceiveNext();
            float w = (float)stream.ReceiveNext();
            headRot = new Quaternion(x, y, z, w);
        }
    }
}
