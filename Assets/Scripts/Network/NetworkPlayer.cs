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
    [SerializeField] private SkinnedMeshRenderer faceMesh, teethMesh;
    [SerializeField] private Transform lArmTf, rArmTf, headTf;

    [HideInInspector] public Quaternion headRot;
    [HideInInspector] public bool lArmMotion, rArmMotion;
    [HideInInspector] public ArrayList lRots, rRots;

    void Start()
    {
        if (PhotonNetwork.IsConnected && !photonView.IsMine)
        {
            playerFollowCam.SetActive(false);

            GetComponent<CharacterController>().enabled = false;
            GetComponent<PlayerInput>().enabled = false;
            GetComponent<ThirdPersonController>().enabled = false;
            GetComponent<BasicRigidBodyPush>().enabled = false;
            GetComponent<StarterAssetsInputs>().enabled = false;
        }

        // this client uses these to send info over network
        // other clients uses these to receive info and overwrite animator
        headRot = headTf.localRotation;
        lArmMotion = false;
        rArmMotion = false;
        lRots = new ArrayList();
        rRots = new ArrayList();
    }

    void LateUpdate()
    {
        if (!photonView.IsMine) 
        {
            headTf.localRotation = headRot;
            if (lArmMotion) setJointRotations(lArmTf, lRots, 0);
            if (rArmMotion) setJointRotations(rArmTf, rRots, 0);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // facial expression
        int nBlendshapes = faceMesh.sharedMesh.blendShapeCount;
        for (int i = 0; i < nBlendshapes; i++)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(faceMesh.GetBlendShapeWeight(i));
            }
            else
            {
                float weight = (float)stream.ReceiveNext();
                faceMesh.SetBlendShapeWeight(i, weight);
                if (faceMesh.sharedMesh.GetBlendShapeName(i) == "jawOpen")
                {
                    teethMesh.SetBlendShapeWeight(i, weight);
                }
            }
        }

        // head
        if (stream.IsWriting) stream.SendNext(headRot);
        else headRot = (Quaternion)stream.ReceiveNext();

        // arms
        if (stream.IsWriting)
        {
            if (lArmMotion)
            {
                stream.SendNext(lRots.Count);
                foreach (Quaternion rot in lRots) stream.SendNext(rot);
            }
            else
            {
                stream.SendNext(0);
            }

            if (rArmMotion)
            {
                stream.SendNext(rRots.Count);
                foreach (Quaternion rot in rRots) stream.SendNext(rot);
            }
            else
            {
                stream.SendNext(0);
            }
        }
        else
        {
            int jointCount = (int)stream.ReceiveNext();
            if (jointCount > 0)
            {
                lRots.Clear();
                for (int i = 0; i < jointCount; i++) lRots.Add((Quaternion)stream.ReceiveNext());
                lArmMotion = true;
            }
            else
            {
                lArmMotion = false;
            }

            jointCount = (int)stream.ReceiveNext();
            if (jointCount > 0)
            {
                rRots.Clear();
                for (int i = 0; i < jointCount; i++) rRots.Add((Quaternion)stream.ReceiveNext());
                rArmMotion = true;
            }
            else
            {
                rArmMotion = false;
            }
        }
    }

    private int setJointRotations(Transform joint, ArrayList rots, int idx)
    {
        joint.localRotation = (Quaternion)rots[idx++];
        foreach (Transform child in joint) 
        {
            idx = setJointRotations(child, rots, idx);
        }
        return idx;
    }
}
