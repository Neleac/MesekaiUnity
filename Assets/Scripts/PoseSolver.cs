using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;

public class PoseSolver : MonoBehaviour
{
    const float SMOOTHING = 0.1f;   // lower value = smoother, but less responsive

    // landmark indices
    const int LEFTSHOULDER = 11;
    const int RIGHTSHOULDER = 12;
    const int LEFTELBOW = 13;
    const int RIGHTELBOW = 14;
    const int LEFTWRIST = 15;
    const int RIGHTWRIST = 16;
    const int LEFTFINGER = 19;
    const int RIGHTFINGER = 20;

    private Transform lShoulderTf;
    private Transform lElbowTf;
    private Transform lWristTf;
    private Transform lFingerTf;
    private Transform rShoulderTf;
    private Transform rElbowTf;
    private Transform rWristTf;
    private Transform rFingerTf;

    private LandmarkList poseLandmarks;

    [SerializeField] private Transform hips;
    [SerializeField] private bool mirrorMode;

    void Start()
    {
        Transform spine2 = hips.Find("Spine").Find("Spine1").Find("Spine2");

        lShoulderTf = spine2.Find("LeftShoulder").Find("LeftArm");
        lElbowTf = lShoulderTf.Find("LeftForeArm");
        lWristTf = lElbowTf.Find("LeftHand");
        lFingerTf = lWristTf.Find("LeftHandIndex1");

        rShoulderTf = spine2.Find("RightShoulder").Find("RightArm");
        rElbowTf = rShoulderTf.Find("RightForeArm");
        rWristTf = rElbowTf.Find("RightHand");
        rFingerTf = rWristTf.Find("RightHandIndex1");

        if (!mirrorMode)
        {
            (lShoulderTf, rShoulderTf) = (rShoulderTf, lShoulderTf);
            (lElbowTf, rElbowTf) = (rElbowTf, lElbowTf);
            (lWristTf, rWristTf) = (rWristTf, lWristTf);
            (lFingerTf, rFingerTf) = (rFingerTf, lFingerTf);
        }

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
        /*
        naming: 
            Lm denotes Landmark
            Tf denotes Transform
            v_ denotes vector
        */

        Vector3 lShoulderLm = new Vector3(-poseLandmarks.Landmark[LEFTSHOULDER].X, -poseLandmarks.Landmark[LEFTSHOULDER].Y, poseLandmarks.Landmark[LEFTSHOULDER].Z);
        Vector3 rShoulderLm = new Vector3(-poseLandmarks.Landmark[RIGHTSHOULDER].X, -poseLandmarks.Landmark[RIGHTSHOULDER].Y, poseLandmarks.Landmark[RIGHTSHOULDER].Z);
        if (!mirrorMode)
        {
            lShoulderLm.z *= -1;
            rShoulderLm.z *= -1;
        }
        
        Vector3 v_ShoulderLm = (rShoulderLm - lShoulderLm).normalized;
        Vector3 v_ShoulderTf = (lShoulderTf.position - rShoulderTf.position).normalized;
        Quaternion rot = Quaternion.FromToRotation(v_ShoulderLm, v_ShoulderTf);

        // convert Landmarks to Vector3s, with shoulders aligned with avatar
        Vector3[] landmarks = new Vector3[poseLandmarks.Landmark.Count];
        for (int i = 0; i < landmarks.Length; i++)
        {
            /*
            landmark coordinate frame
               x-axis: left to right
               y-axis: bottom to top
               z-axis: into the screen
            */
            landmarks[i] = new Vector3(-poseLandmarks.Landmark[i].X, -poseLandmarks.Landmark[i].Y, poseLandmarks.Landmark[i].Z);
            if (!mirrorMode) landmarks[i].z *= -1;

            landmarks[i] = rot * landmarks[i];
        }

        // limbs
        (Transform, Transform)[] transformPairs = { (rShoulderTf, rElbowTf), (rElbowTf, rWristTf), (rWristTf, rFingerTf),
                                                    (lShoulderTf, lElbowTf), (lElbowTf, lWristTf), (lWristTf, lFingerTf) };
        (int, int)[] landmarkIdxPairs = { (LEFTSHOULDER, LEFTELBOW), (LEFTELBOW, LEFTWRIST), (LEFTWRIST, LEFTFINGER),
                                          (RIGHTSHOULDER, RIGHTELBOW), (RIGHTELBOW, RIGHTWRIST), (RIGHTWRIST, RIGHTFINGER) };
        Debug.Assert(transformPairs.Length == landmarkIdxPairs.Length);

        // solve rotations
        for (int i = 0; i < transformPairs.Length; i++)
        {
            (Transform parentTf, Transform childTf) = transformPairs[i];
            (int parentLmIdx, int childLmIdx) = landmarkIdxPairs[i];
            
            // current avatar limb direction, in joint Transform's local space
            Vector3 vOld = (childTf.position - parentTf.position).normalized;
            vOld = parentTf.InverseTransformDirection(vOld);

            // current player limb direction, in joint Transform's local space
            Vector3 vNew = (landmarks[childLmIdx] - landmarks[parentLmIdx]).normalized;
            vNew = parentTf.InverseTransformDirection(vNew);

            // smooth, interpolated rotation
            Quaternion rotOld = parentTf.localRotation;
            Quaternion rotNew = rotOld * Quaternion.FromToRotation(vOld, vNew);
            parentTf.localRotation = Quaternion.Slerp(rotOld, rotNew, SMOOTHING);
        }
    }
}