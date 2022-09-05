using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTransfer : MonoBehaviour
{
    [SerializeField] private Transform lArmTemplate, rArmTemplate, headTemplate;
    [SerializeField] private SkinnedMeshRenderer faceMeshTemplate;

    [HideInInspector] public GameObject playerAvatar;
    private Transform lArmPlayer, rArmPlayer, headPlayer;
    private SkinnedMeshRenderer faceMeshPlayer;

    void Start()
    {
        Transform spine2Player = playerAvatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2");
        lArmPlayer = spine2Player.Find("LeftShoulder/LeftArm");
        rArmPlayer = spine2Player.Find("RightShoulder/RightArm");
        headPlayer = spine2Player.Find("Neck/Head");
        faceMeshPlayer = playerAvatar.transform.Find("Avatar_Renderer_Head").GetComponent<SkinnedMeshRenderer>();
    }

    void LateUpdate()
    {
        NetworkPlayer networkPlayer = playerAvatar.GetComponent<NetworkPlayer>();
        
        if (GetComponent<HandSolver>().leftDetected)
        {
            networkPlayer.lRots.Clear();
            mapJointRotation(lArmPlayer, lArmTemplate, networkPlayer.lRots);
            networkPlayer.lArmMotion = true;
        }
        else
        {
            networkPlayer.lArmMotion = false;
        }

        if (GetComponent<HandSolver>().rightDetected)
        {
            networkPlayer.rRots.Clear();
            mapJointRotation(rArmPlayer, rArmTemplate, networkPlayer.rRots);
            networkPlayer.rArmMotion = true;
        }
        else
        {
            networkPlayer.rArmMotion = false;
        }
        
        headPlayer.localRotation = headTemplate.localRotation;
        networkPlayer.headRot = headTemplate.localRotation;

        for (int i = 0; i < faceMeshTemplate.sharedMesh.blendShapeCount; i++)
        {
            float weight = faceMeshTemplate.GetBlendShapeWeight(i);
            faceMeshPlayer.SetBlendShapeWeight(i, weight);
        }
    }

    private void mapJointRotation(Transform playerJoint, Transform templateJoint, ArrayList rots)
    {       
        playerJoint.localRotation = templateJoint.localRotation;
        rots.Add(playerJoint.localRotation);

        foreach (Transform playerChild in playerJoint)
        {
            Transform templateChild = templateJoint.Find(playerChild.name);
            mapJointRotation(playerChild, templateChild, rots);
        }
    }
}
