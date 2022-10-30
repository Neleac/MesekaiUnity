using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MotionTransfer : MonoBehaviour
{
    // parent joints of all joints to be retargeted
    public Transform[] srcJoints, tgtJoints;
    
    // blendshape meshes
    [SerializeField] private SkinnedMeshRenderer srcFace;
    public SkinnedMeshRenderer tgtFace, tgtTeeth;

    void Update()
    {
        // transfer face/teeth blendshapes
        for (int i = 0; i < srcFace.sharedMesh.blendShapeCount; i++)
        {
            float weight = srcFace.GetBlendShapeWeight(i);
            string name = srcFace.sharedMesh.GetBlendShapeName(i);

            int idx = tgtFace.sharedMesh.GetBlendShapeIndex(name);
            if (idx != -1) tgtFace.SetBlendShapeWeight(idx, weight);

            if (name == "jawOpen")
            {
                tgtTeeth.SetBlendShapeWeight(idx, weight);
            }
        }
    }

    void LateUpdate()
    {
        // transfer joint rotations
        for (int i = 0; i < srcJoints.Length; i++)
        {
            mapJointRotation(srcJoints[i], tgtJoints[i]);
        }
    }

    private void mapJointRotation(Transform src, Transform tgt)
    {       
        tgt.localRotation = src.localRotation;

        foreach (Transform srcChild in src)
        {
            Transform tgtChild = tgt.Find(srcChild.name);
            mapJointRotation(srcChild, tgtChild);
        }
    }
}
