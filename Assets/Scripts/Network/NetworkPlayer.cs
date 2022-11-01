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

    // src: template avatar, sends data over network
    [HideInInspector] public SkinnedMeshRenderer srcFace;
    [HideInInspector] public Transform srcLArm, srcRArm, srcHead;

    // tgt: default or custom avatar, receives data from src client
    public SkinnedMeshRenderer tgtFace, tgtTeeth;
    public Transform tgtLArm, tgtRArm, tgtHead;

    // stores received joint rots, used in LateUpdate to override animator
    private ArrayList tgtLArmRots, tgtRArmRots;
    private Quaternion tgtHeadRot;

    private Vector3 position;
    [HideInInspector] public bool idle;

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

        tgtLArmRots = new ArrayList();
        tgtRArmRots = new ArrayList();

        position = transform.position;
        idle = true;
    }

    void Update()
    {
        idle = transform.position == position;
        position = transform.position;
    }

    void LateUpdate()
    {
        if (!photonView.IsMine) 
        {
            if (tgtLArmRots.Count > 0) SetRots(tgtLArm, tgtLArmRots, 0);
            if (tgtRArmRots.Count > 0) SetRots(tgtRArm, tgtRArmRots, 0);
            tgtHead.localRotation = tgtHeadRot;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // facial expression
        int nBlendshapes = tgtFace.sharedMesh.blendShapeCount;
        for (int i = 0; i < nBlendshapes; i++)
        {
            if (stream.IsWriting)   // send
            {
                stream.SendNext(srcFace.GetBlendShapeWeight(i));
            }
            else    // receive
            {
                float weight = (float)stream.ReceiveNext();
                tgtFace.SetBlendShapeWeight(i, weight);
                if (tgtFace.sharedMesh.GetBlendShapeName(i) == "jawOpen")
                {
                    tgtTeeth.SetBlendShapeWeight(i, weight);
                }
            }
        }

        // joint rotations
        if (stream.IsWriting)   // send
        {
            if (idle)
            {
                stream.SendNext(true);
                SendRots(stream, srcLArm);
                SendRots(stream, srcRArm);
            }
            else
            {
                stream.SendNext(false);
            }

            stream.SendNext(srcHead.localRotation);
        }
        else    // receive
        {
            tgtLArmRots.Clear();
            tgtRArmRots.Clear();
            if ((bool)stream.ReceiveNext())
            {
                ReceiveRots(stream, tgtLArm, tgtLArmRots);
                ReceiveRots(stream, tgtRArm, tgtRArmRots);
            }

            tgtHeadRot = (Quaternion)stream.ReceiveNext();
        }
    }

    private void SendRots(PhotonStream stream, Transform joint)
    {
        stream.SendNext(joint.localRotation);
        foreach (Transform child in joint)
        {
            SendRots(stream, child);
        }
    }

    private void ReceiveRots(PhotonStream stream, Transform joint, ArrayList rots)
    {
        rots.Add((Quaternion)stream.ReceiveNext());
        foreach (Transform child in joint)
        {
            ReceiveRots(stream, child, rots);
        } 
    }

    private int SetRots(Transform joint, ArrayList rots, int idx)
    {
        joint.localRotation = (Quaternion)rots[idx++];
        foreach (Transform child in joint) 
        {
            idx = SetRots(child, rots, idx);
        }
        return idx;
    }
}
