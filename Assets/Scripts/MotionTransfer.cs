using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MotionTransfer : MonoBehaviour
{
    public GameObject playerAvatar;

    [SerializeField] private Transform lArmTemplate, rArmTemplate, headTemplate;
    [SerializeField] private SkinnedMeshRenderer faceMeshTemplate;
    
    private Transform lArmPlayer, rArmPlayer, headPlayer;
    private SkinnedMeshRenderer faceMeshPlayer, teethMeshPlayer;
    private string sceneName;

    void Start()
    {
        Transform spine2Player = playerAvatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2");
        lArmPlayer = spine2Player.Find("LeftShoulder/LeftArm");
        rArmPlayer = spine2Player.Find("RightShoulder/RightArm");
        headPlayer = spine2Player.Find("Neck/Head");
        faceMeshPlayer = playerAvatar.transform.Find("Avatar_Renderer_Head").GetComponent<SkinnedMeshRenderer>();
        teethMeshPlayer = playerAvatar.transform.Find("Avatar_Renderer_Teeth").GetComponent<SkinnedMeshRenderer>();

        sceneName = SceneManager.GetActiveScene().name;
    }

    // only runs in Avatar Mirror, maps from template to custom RPM avatar
    void Update()
    {
        if (sceneName == "Avatar")
        {
            mapJointRotation(lArmPlayer, lArmTemplate, null);
            mapJointRotation(rArmPlayer, rArmTemplate, null);

            headPlayer.localRotation = headTemplate.localRotation;

            for (int i = 0; i < faceMeshTemplate.sharedMesh.blendShapeCount; i++)
            {
                float weight = faceMeshTemplate.GetBlendShapeWeight(i);
                faceMeshPlayer.SetBlendShapeWeight(i, weight);

                if (faceMeshTemplate.sharedMesh.GetBlendShapeName(i) == "jawOpen")
                {
                    teethMeshPlayer.SetBlendShapeWeight(i, weight);
                }
            }
        }
    }

    // only runs in Hub World, stores info in NetworkPlayer to be sent over network 
    void LateUpdate()
    {
        if (sceneName == "Hub")
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

                if (faceMeshTemplate.sharedMesh.GetBlendShapeName(i) == "jawOpen")
                {
                    teethMeshPlayer.SetBlendShapeWeight(i, weight);
                }
            }
        }
    }

    private void mapJointRotation(Transform playerJoint, Transform templateJoint, ArrayList rots)
    {       
        playerJoint.localRotation = templateJoint.localRotation;

        if (rots != null) rots.Add(playerJoint.localRotation);

        foreach (Transform playerChild in playerJoint)
        {
            Transform templateChild = templateJoint.Find(playerChild.name);
            mapJointRotation(playerChild, templateChild, rots);
        }
    }
}
