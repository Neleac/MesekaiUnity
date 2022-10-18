using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MotionTransfer : MonoBehaviour
{
    public GameObject playerAvatar;
    public Transform lArmPlayer, rArmPlayer, headPlayer;
    public Transform spinePlayer, spine1Player, spine2Player;
    public SkinnedMeshRenderer faceMeshPlayer, teethMeshPlayer;

    [SerializeField] private Transform lArmTemplate, rArmTemplate, headTemplate, spineTemplate;
    [SerializeField] private SkinnedMeshRenderer faceMeshTemplate;

    private string sceneName;

    void Start()
    {
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
            spinePlayer.localRotation = spineTemplate.localRotation;
            spine1Player.localRotation = spineTemplate.localRotation;
            spine2Player.localRotation = spineTemplate.localRotation;

            for (int i = 0; i < faceMeshTemplate.sharedMesh.blendShapeCount; i++)
            {
                float weight = faceMeshTemplate.GetBlendShapeWeight(i);
                string name = faceMeshTemplate.sharedMesh.GetBlendShapeName(i);

                int idx = faceMeshPlayer.sharedMesh.GetBlendShapeIndex(name);
                if (idx != -1) faceMeshPlayer.SetBlendShapeWeight(idx, weight);

                if (name == "jawOpen")
                {
                    teethMeshPlayer.SetBlendShapeWeight(idx, weight);
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
