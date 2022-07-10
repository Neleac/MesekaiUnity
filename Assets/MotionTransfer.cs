using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTransfer : MonoBehaviour
{
    [SerializeField] private Transform lShoulderTemplate, rShoulderTemplate, headTemplate;
    [SerializeField] private SkinnedMeshRenderer faceMeshTemplate;

    [HideInInspector] public GameObject playerAvatar;
    private Transform lShoulderPlayer, rShoulderPlayer, headPlayer;
    private SkinnedMeshRenderer faceMeshPlayer;

    void Start()
    {
        lShoulderPlayer = playerAvatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2/LeftShoulder");
        rShoulderPlayer = playerAvatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2/RightShoulder");
        headPlayer = playerAvatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2/Neck/Head");
        faceMeshPlayer = playerAvatar.transform.Find("Avatar_Renderer_Head").GetComponent<SkinnedMeshRenderer>();
    }

    void LateUpdate()
    {
        if (GetComponent<HandSolver>().leftDetected) mapJointRotation(lShoulderPlayer, lShoulderTemplate);
        if (GetComponent<HandSolver>().rightDetected) mapJointRotation(rShoulderPlayer, rShoulderTemplate);

        headPlayer.localRotation = headTemplate.localRotation;

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
