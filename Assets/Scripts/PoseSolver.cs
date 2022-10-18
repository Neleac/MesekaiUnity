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

    private Transform spine, spine1, spine2;
    private Transform lShoulderTf, lElbowTf, lWristTf, lFingerTf;
    private Transform rShoulderTf, rElbowTf, rWristTf, rFingerTf;

    private (Transform, Transform)[] transformPairs;
    private (int, int)[] landmarkIdxPairs;

    private LandmarkList poseLandmarks;
    private Quaternion[] prevRots;

    [SerializeField] private Transform hips;
    [SerializeField] private bool mirrorMode;

    void Start()
    {
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

        if (!mirrorMode)
        {
            (lShoulderTf, rShoulderTf) = (rShoulderTf, lShoulderTf);
            (lElbowTf, rElbowTf) = (rElbowTf, lElbowTf);
            (lWristTf, rWristTf) = (rWristTf, lWristTf);
            (lFingerTf, rFingerTf) = (rFingerTf, lFingerTf);
        }

        transformPairs = new (Transform, Transform)[]{ (rShoulderTf, rElbowTf), (rElbowTf, rWristTf), (rWristTf, rFingerTf),
                                                       (lShoulderTf, lElbowTf), (lElbowTf, lWristTf), (lWristTf, lFingerTf) };
        landmarkIdxPairs = new (int, int)[]{ (LEFTSHOULDER, LEFTELBOW), (LEFTELBOW, LEFTWRIST), (LEFTWRIST, LEFTFINGER),
                                             (RIGHTSHOULDER, RIGHTELBOW), (RIGHTELBOW, RIGHTWRIST), (RIGHTWRIST, RIGHTFINGER) };

        poseLandmarks = null;

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
        Quaternion rotInv = Quaternion.Inverse(rot);

        spine.localRotation = Quaternion.Slerp(spine.localRotation, spine.localRotation * rotInv, SMOOTHING);
        spine1.localRotation = Quaternion.Slerp(spine1.localRotation, spine1.localRotation * rotInv, SMOOTHING);
        spine2.localRotation = Quaternion.Slerp(spine2.localRotation, spine2.localRotation * rotInv, SMOOTHING);
        
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
}
