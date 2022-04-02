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

    private void SolvePose()
    {
        // TODO: use poseLandmarks to solve for joint rotations

        // shoulder rotation example, see Transform for more info:
        // https://docs.unity3d.com/ScriptReference/Transform.html
        // leftShoulder.localEulerAngles = new Vector3(30, 45, 90);
        Vector3 direction = toAngle(poseLandmarks.Landmark[11], poseLandmarks.Landmark[13]);

        // Explain
        // position from the camera has the opposite value as in the unity world.
        direction.x = -direction.x;
        direction.y = -direction.y;
        // direction.z = 0;


        // print(poseLandmarks.Landmark[16].ToString());
        //print(poseLandmarks.Landmark[15].ToString());
        //print(direction);
        // Vector3 euAngle = Quaternion.LookRotation(leftShoulder.InverseTransformDirection(direction)).eulerAngles;


        // Explain: 
        // 1. direction is the result I calculated from camera.
        // 2. new Vector3(0, 1, 1) is the direction of the left upper arm.
        // 3. FromToRotation rotate the item to keep the first direction align with the second direcion.
        Vector3 euAngle = Quaternion.FromToRotation(new Vector3(0, 1, 1), direction).eulerAngles;
        // Vector3 euAngle = Quaternion.FromToRotation(Vector3.up, direction).eulerAngles;
        // Vector3 euAngle = Quaternion.FromToRotation(new Vector3(92.358f, -105.363f, -14.85901f), direction).eulerAngles;

        // print(euAngle);
        leftShoulder.localEulerAngles = (euAngle);
    }

    private Vector3 toAngle(Landmark from, Landmark to)
    {
        Vector3 direction = new Vector3(to.X - from.X, to.Y - from.Y, to.Z - from.Z);
        return direction;
        // return direction.normalized;
    }
}