using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;

public class PoseSolver : MonoBehaviour
{
    private Transform armature;
    private Transform spine2;
    private Transform leftShoulder;

    private LandmarkList poseLandmarks;

    void Start()
    {
        armature = transform.Find("Hips");
        spine2 = armature.Find("Spine").Find("Spine1").Find("Spine2");
        leftShoulder = spine2.Find("LeftShoulder");

        poseLandmarks = null;
    }

    void Update()
    {
        if (poseLandmarks != null) SolvePose();
    }

    // called from HolisticTrackingSolution.cs
    public void SetPoseLandmarks(LandmarkList poseWorldLandmarks)
    {
        poseLandmarks = poseWorldLandmarks;
    }

    private void SolvePose() {
        // TODO: use poseLandmarks to solve for joint rotations

        // shoulder rotation example, see Transform for more info:
        // https://docs.unity3d.com/ScriptReference/Transform.html
        leftShoulder.localEulerAngles = new Vector3(0, 0, 90);
    }
}
