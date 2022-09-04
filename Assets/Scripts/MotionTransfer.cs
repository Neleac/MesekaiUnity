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
        if (GetComponent<HandSolver>().leftDetected) mapJointRotation(lArmPlayer, lArmTemplate);
        if (GetComponent<HandSolver>().rightDetected) mapJointRotation(rArmPlayer, rArmTemplate);

        headPlayer.localRotation = headTemplate.localRotation;
        playerAvatar.GetComponent<NetworkPlayer>().headRot = headTemplate.localRotation;

        for (int i = 0; i < faceMeshTemplate.sharedMesh.blendShapeCount; i++)
        {
            float weight = faceMeshTemplate.GetBlendShapeWeight(i);
            faceMeshPlayer.SetBlendShapeWeight(i, weight);
        }
    }

    private void mapJointRotation(Transform playerJoint, Transform templateJoint)
    {       
        playerJoint.localRotation = templateJoint.localRotation;

        foreach (Transform playerChild in playerJoint)
        {
            Transform templateChild = templateJoint.Find(playerChild.name);
            mapJointRotation(playerChild, templateChild);
        }
    }
}
