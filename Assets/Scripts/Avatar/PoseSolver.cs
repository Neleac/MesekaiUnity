using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;

public class PoseSolver : MonoBehaviour
{
    const float SMOOTHING = 0.1f;   // lower value = smoother, but less responsive

    // landmark indices
    const int L_SHOULDER = 11;
    const int R_SHOULDER = 12;
    const int L_ELBOW = 13;
    const int R_ELBOW = 14;
    const int L_WRIST = 15;
    const int R_WRIST = 16;
    const int L_FINGER = 19;
    const int R_FINGER = 20;

    const int L_HIP = 23;
    const int R_HIP = 24;
    const int L_KNEE = 25;
    const int R_KNEE = 26;
    const int L_ANKLE = 27;
    const int R_ANKLE = 28;
    const int L_TOE = 31;
    const int R_TOE = 32;

    private Transform spine, spine1, spine2;
    private Transform lShoulderTf, lElbowTf, lWristTf, lFingerTf;
    private Transform rShoulderTf, rElbowTf, rWristTf, rFingerTf;
    private Transform lHipTf, lKneeTf, lAnkleTf, lToeTf;
    private Transform rHipTf, rKneeTf, rAnkleTf, rToeTf;

    private (Transform, Transform)[] transformPairs, legTfPairs;
    private (int, int)[] landmarkIdxPairs, legLmIdxPairs;

    private LandmarkList poseLandmarks;
    private Quaternion[] prevRots;

    [SerializeField] private Transform hips;
    [SerializeField] private bool mirrorMode;

    void Start()
    {
        poseLandmarks = null;

        spine = hips.Find("Spine");
        spine1 = spine.Find("Spine1");
        spine2 = spine1.Find("Spine2");
        lShoulderTf = spine2.Find("LeftShoulder/LeftArm");
        lElbowTf = lShoulderTf.Find("LeftForeArm");
        lWristTf = lElbowTf.Find("LeftHand");
        lFingerTf = lWristTf.Find("LeftHandIndex1");
        rShoulderTf = spine2.Find("RightShoulder/RightArm");
        rElbowTf = rShoulderTf.Find("RightForeArm");
        rWristTf = rElbowTf.Find("RightHand");
        rFingerTf = rWristTf.Find("RightHandIndex1");
        lHipTf = hips.Find("LeftUpLeg");
        lKneeTf = lHipTf.Find("LeftLeg");
        lAnkleTf = lKneeTf.Find("LeftFoot");
        lToeTf = lAnkleTf.Find("LeftToeBase");
        rHipTf = hips.Find("RightUpLeg");
        rKneeTf = rHipTf.Find("RightLeg");
        rAnkleTf = rKneeTf.Find("RightFoot");
        rToeTf = rAnkleTf.Find("RightToeBase");

        if (!mirrorMode)
        {
            (lShoulderTf, rShoulderTf) = (rShoulderTf, lShoulderTf);
            (lElbowTf, rElbowTf) = (rElbowTf, lElbowTf);
            (lWristTf, rWristTf) = (rWristTf, lWristTf);
            (lFingerTf, rFingerTf) = (rFingerTf, lFingerTf);
        }

        transformPairs = new (Transform, Transform)[]{ (rShoulderTf, rElbowTf), (rElbowTf, rWristTf), (rWristTf, rFingerTf),
                                                       (lShoulderTf, lElbowTf), (lElbowTf, lWristTf), (lWristTf, lFingerTf),
                                                       (rHipTf, rKneeTf), (rKneeTf, rAnkleTf), (rAnkleTf, rToeTf),
                                                       (lHipTf, lKneeTf), (lKneeTf, lAnkleTf), (lAnkleTf, lToeTf) };

        landmarkIdxPairs = new (int, int)[]{ (L_SHOULDER, L_ELBOW), (L_ELBOW, L_WRIST), (L_WRIST, L_FINGER),
                                             (R_SHOULDER, R_ELBOW), (R_ELBOW, R_WRIST), (R_WRIST, R_FINGER),
                                             (L_HIP, L_KNEE), (L_KNEE, L_ANKLE), (L_ANKLE, L_TOE),
                                             (R_HIP, R_KNEE), (R_KNEE, R_ANKLE), (R_ANKLE, R_TOE) };

        prevRots = new Quaternion[transformPairs.Length];
        for (int i = 0; i < prevRots.Length; i++)
        {
            prevRots[i] = transformPairs[i].Item1.localRotation;
        }
    }

    void Update()
    {
        for (int i = 0; i < transformPairs.Length; i++)
        {
            (Transform parentTf, Transform childTf) = transformPairs[i];
            parentTf.localRotation = Quaternion.identity;
        }
        spine.localRotation = Quaternion.identity;
        spine1.localRotation = Quaternion.identity;
        spine2.localRotation = Quaternion.identity;

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

        Vector3 lShoulderLm = new Vector3(-poseLandmarks.Landmark[L_SHOULDER].X, -poseLandmarks.Landmark[L_SHOULDER].Y, poseLandmarks.Landmark[L_SHOULDER].Z);
        Vector3 rShoulderLm = new Vector3(-poseLandmarks.Landmark[R_SHOULDER].X, -poseLandmarks.Landmark[R_SHOULDER].Y, poseLandmarks.Landmark[R_SHOULDER].Z);
        Vector3 lHipLm = new Vector3(-poseLandmarks.Landmark[L_HIP].X, -poseLandmarks.Landmark[L_HIP].Y, poseLandmarks.Landmark[L_HIP].Z);
        Vector3 rHipLm = new Vector3(-poseLandmarks.Landmark[R_HIP].X, -poseLandmarks.Landmark[R_HIP].Y, poseLandmarks.Landmark[R_HIP].Z);
        
        if (!mirrorMode)
        {
            lShoulderLm.z *= -1;
            rShoulderLm.z *= -1;
        }
        
        Vector3 v_ShoulderLm = (rShoulderLm - lShoulderLm).normalized;
        Vector3 v_ShoulderTf = (lShoulderTf.position - rShoulderTf.position).normalized;
        Vector3 v_HipLm = (rHipLm - lHipLm).normalized;
        Vector3 v_HipTf = (lHipTf.position - rHipTf.position).normalized;
        
        // spine
        Quaternion rot = Quaternion.FromToRotation(v_ShoulderTf, v_ShoulderLm);
        spine.localRotation = Quaternion.Slerp(spine.localRotation, spine.localRotation * rot, SMOOTHING);
        spine1.localRotation = Quaternion.Slerp(spine1.localRotation, spine1.localRotation * rot, SMOOTHING);
        spine2.localRotation = Quaternion.Slerp(spine2.localRotation, spine2.localRotation * rot, SMOOTHING);
        
        // convert Landmarks to Vector3s, with shoulders aligned with avatar
        Vector3[] landmarks = new Vector3[poseLandmarks.Landmark.Count];
        AlignLandmarks(v_ShoulderLm, v_ShoulderTf, landmarks, L_SHOULDER, R_FINGER);
        AlignLandmarks(v_HipLm, v_HipTf, landmarks, L_HIP, R_TOE);

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
            Quaternion rotOld = prevRots[i];
            Quaternion rotNew = parentTf.localRotation * Quaternion.FromToRotation(vOld, vNew).normalized;
            parentTf.localRotation = Quaternion.Slerp(rotOld, rotNew, SMOOTHING);
            prevRots[i] = parentTf.localRotation;
        }
    }

    private void AlignLandmarks(Vector3 from, Vector3 to, Vector3[] landmarks, int startIdx, int endIdx)
    {
        Quaternion rot = Quaternion.FromToRotation(from, to);
        for (int i = startIdx; i <= endIdx; i++)
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
    }
}
