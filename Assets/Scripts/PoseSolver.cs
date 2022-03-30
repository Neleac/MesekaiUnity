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
    private NormalizedLandmarkList leftHandLandmarks;

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
        if (leftHandLandmarks != null) SolveLeftHand();
    }

    // called from HolisticTrackingSolution.cs
    public void SetPoseLandmarks(LandmarkList poseWorldLandmarks)
    {
        poseLandmarks = poseWorldLandmarks;
    }

    public void SetLeftHandLandmarks(NormalizedLandmarkList leftHandLandmarks)
    {
        this.leftHandLandmarks = leftHandLandmarks;
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

    private void SolveLeftHand()
    {
        //for now, the result is shown in the console --YT Mar 29, 2022
        NormalizedLandmark wrist = leftHandLandmarks.Landmark[0];
        NormalizedLandmark indexTip = leftHandLandmarks.Landmark[8];
        NormalizedLandmark indexMCP = leftHandLandmarks.Landmark[5];
        // a closed finger tip is placed between the center of the palm and the wrist
        // y value is used when the hand is upright, x value is used when hand is horizontal
        bool indexClosed = (wrist.X - indexTip.X) * (indexMCP.X - indexTip.X) < -0.001 || (wrist.Y - indexTip.Y) * (indexMCP.Y - indexTip.Y) < -0.001;

        NormalizedLandmark middleTip = leftHandLandmarks.Landmark[12];
        NormalizedLandmark middleMCP = leftHandLandmarks.Landmark[9];
        bool middleClosed = (wrist.X - middleTip.X) * (middleMCP.X - middleTip.X) < -0.001 || (wrist.Y - middleTip.Y) * (middleMCP.Y - middleTip.Y) < -0.001;

        NormalizedLandmark ringTip = leftHandLandmarks.Landmark[16];
        NormalizedLandmark ringMCP = leftHandLandmarks.Landmark[13];
        bool ringClosed = (wrist.X - ringTip.X) * (ringMCP.X - ringTip.X) < -0.001 || (wrist.Y - ringTip.Y) * (ringMCP.Y - ringTip.Y) < -0.001;
        //print((wrist.X - ringTip.X) * (ringMCP.X - ringTip.X));
        //print((wrist.Y - ringTip.Y) * (ringMCP.Y - ringTip.Y));

        NormalizedLandmark pinkyTip = leftHandLandmarks.Landmark[20];
        NormalizedLandmark pinkyMCP = leftHandLandmarks.Landmark[17];
        bool pinkyClosed = (wrist.X - pinkyTip.X) * (pinkyMCP.X - pinkyTip.X) < -0.001 || (wrist.Y - pinkyTip.Y) * (pinkyMCP.Y - pinkyTip.Y) < -0.001;


        // print(indexClosed + ", " + indexTip + ", " + indexMCP + ", " + wrist);

        print(indexClosed + ", " + middleClosed  + ", " + ringClosed + ", " + pinkyClosed);
        if (indexClosed && middleClosed && ringClosed && pinkyClosed)
        {
            print("rock");
        } else if (!indexClosed && !middleClosed && ringClosed && pinkyClosed)
        {
            print("scissor");
        } else if (!indexClosed && !middleClosed && !ringClosed && !pinkyClosed)
        {
            print("paper");
        } else
        {
            print("N/A");
        }

    }
}