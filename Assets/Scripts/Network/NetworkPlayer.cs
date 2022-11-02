using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;
using StarterAssets;
using ReadyPlayerMe;

public class NetworkPlayer : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback
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

    private AvatarLoader avatarLoader;

    void Awake()
    {
        avatarLoader = GameObject.Find("Avatar Loader").GetComponent<RPMAvatarLoader>().avatarLoader;
    }

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

        GameObject templateAvatar = GameObject.Find("Template Avatar");
        srcFace = templateAvatar.transform.Find("Avatar_Renderer_Head").GetComponent<SkinnedMeshRenderer>();
        Transform spine = templateAvatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2");
        srcLArm = spine.Find("LeftShoulder/LeftArm");
        srcRArm = spine.Find("RightShoulder/RightArm");
        srcHead = spine.Find("Neck/Head");

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
            if (tgtLArmRots.Count > 0) SetRots(srcLArm, tgtLArm, tgtLArmRots, 0);
            if (tgtRArmRots.Count > 0) SetRots(srcRArm, tgtRArm, tgtRArmRots, 0);
            tgtHead.localRotation = tgtHeadRot;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // facial expression
        int nBlendshapes = tgtFace.sharedMesh.blendShapeCount;
        for (int i = 0; i < srcFace.sharedMesh.blendShapeCount; i++)
        {
            if (stream.IsWriting)   // send
            {
                stream.SendNext(srcFace.GetBlendShapeWeight(i));
            }
            else    // receive
            {
                float weight = (float)stream.ReceiveNext();
                string name = srcFace.sharedMesh.GetBlendShapeName(i);

                int idx = tgtFace.sharedMesh.GetBlendShapeIndex(name);
                if (idx != -1) tgtFace.SetBlendShapeWeight(idx, weight);

                if (name == "jawOpen")
                {
                    tgtTeeth.SetBlendShapeWeight(idx, weight);
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

    private int SetRots(Transform src, Transform tgt, ArrayList rots, int idx)
    {
        tgt.localRotation = (Quaternion)rots[idx++];

        foreach (Transform srcChild in src)
        {
            Transform tgtChild = tgt.Find(srcChild.name);
            idx = SetRots(srcChild, tgtChild, rots, idx);
        }
        
        return idx;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (!info.photonView.IsMine)
        {
            object[] instData = info.photonView.InstantiationData;
            if (instData != null)   // client is using custom avatar
            {
                // hide default avatar
                Component[] meshes = GetComponentsInChildren(typeof(SkinnedMeshRenderer));
                foreach (SkinnedMeshRenderer mesh in meshes) mesh.enabled = false;

                string avatarURL = (string)instData[0];
                avatarLoader.LoadAvatar(avatarURL, OnAvatarImported, OnAvatarLoaded);
            }
        }
    }

    private void OnAvatarImported(GameObject avatar)
    {
        Debug.Log($"Avatar imported. [{Time.timeSinceLevelLoad:F2}]");
    }

    private void OnAvatarLoaded(GameObject avatar, AvatarMetaData metaData)
    {
        Debug.Log($"Avatar loaded. [{Time.timeSinceLevelLoad:F2}]\n\n{metaData}");

        // transfer animations
        AnimationTransfer animTransfer = GetComponent<AnimationTransfer>();
        animTransfer.tgt = avatar.GetComponent<Animator>();
        animTransfer.SetController();
        animTransfer.enabled = true;

        // parent to default avatar
        avatar.transform.parent = transform;
        avatar.transform.localPosition = Vector3.zero;
        avatar.transform.localRotation = Quaternion.identity;
        avatar.transform.localScale = Vector3.one;

        // set targets of received blendshapes and rotations
        tgtFace = avatar.transform.Find("Avatar_Renderer_Head").GetComponent<SkinnedMeshRenderer>();
        tgtTeeth = avatar.transform.Find("Avatar_Renderer_Teeth").GetComponent<SkinnedMeshRenderer>();

        Transform spine = avatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2");
        tgtLArm = spine.Find("LeftShoulder/LeftArm");
        tgtRArm = spine.Find("RightShoulder/RightArm");
        tgtHead = spine.Find("Neck/Head");
    }
}
