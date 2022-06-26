using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;

public class HandSolver : MonoBehaviour
{
    // TODO: replace magic numbers with input dimensions from Start/Image Source/Web Cam Source
    const int WIDTH = 640;
    const int HEIGHT = 480;

    const float SMOOTHING = 0.1f;   // lower value = smoother, but less responsive

    // landmark indices
    const int THUMB = 1;
    const int INDEX = 5;
    const int MIDDLE = 9;
    const int RING = 13;
    const int PINKY = 17;
    private int[] landmarkIdxs;
    const int JointsPerFinger = 3;

    private Transform rThumbTf;
    private Transform rIndexTf;
    private Transform rMiddleTf;
    private Transform rRingTf;
    private Transform rPinkyTf;
    private Transform lThumbTf;
    private Transform lIndexTf;
    private Transform lMiddleTf;
    private Transform lRingTf;
    private Transform lPinkyTf;

    private Transform[] lFingerRootTfs;
    private Transform[] rFingerRootTfs;

    private NormalizedLandmarkList leftHandLandmarks;
    private NormalizedLandmarkList rightHandLandmarks;

    private Quaternion[,] prevLeftRots;
    private Quaternion[,] prevRightRots;

    [SerializeField] private Transform hips;
    [SerializeField] private bool mirrorMode;

    void Start()
    {
        landmarkIdxs = new int[]{ THUMB, INDEX, MIDDLE, RING, PINKY };

        Transform spine2 = hips.Find("Spine").Find("Spine1").Find("Spine2");

        Transform lHand = spine2.Find("LeftShoulder").Find("LeftArm").Find("LeftForeArm").Find("LeftHand");
        lThumbTf = lHand.Find("LeftHandThumb1");
        lIndexTf = lHand.Find("LeftHandIndex1");
        lMiddleTf = lHand.Find("LeftHandMiddle1");
        lRingTf = lHand.Find("LeftHandRing1");
        lPinkyTf = lHand.Find("LeftHandPinky1");
        lFingerRootTfs = new Transform[]{ lThumbTf, lIndexTf, lMiddleTf, lRingTf, lPinkyTf };

        Transform rHand = spine2.Find("RightShoulder").Find("RightArm").Find("RightForeArm").Find("RightHand");
        rThumbTf = rHand.Find("RightHandThumb1");
        rIndexTf = rHand.Find("RightHandIndex1");
        rMiddleTf = rHand.Find("RightHandMiddle1");
        rRingTf = rHand.Find("RightHandRing1");
        rPinkyTf = rHand.Find("RightHandPinky1");
        rFingerRootTfs = new Transform[]{ rThumbTf, rIndexTf, rMiddleTf, rRingTf, rPinkyTf };

        if (!mirrorMode)
        {
            (lFingerRootTfs, rFingerRootTfs) = (rFingerRootTfs, lFingerRootTfs);
        }

        leftHandLandmarks = null;
        rightHandLandmarks = null;

        prevLeftRots = new Quaternion[lFingerRootTfs.Length, JointsPerFinger];
        prevRightRots = new Quaternion[rFingerRootTfs.Length, JointsPerFinger];
        for (int i = 0; i < prevLeftRots.GetLength(0); i++)
        {
            Transform lJointTf = lFingerRootTfs[i];
            Transform rJointTf = rFingerRootTfs[i];
            
            for (int j = 0; j < prevLeftRots.GetLength(1); j++)
            {
                prevLeftRots[i, j] = lJointTf.localRotation;
                prevRightRots[i, j] = rJointTf.localRotation;

                lJointTf = lJointTf.GetChild(0);
                rJointTf = rJointTf.GetChild(0);
            }
        }
    }

    void LateUpdate()
    {
        if (leftHandLandmarks != null) SolveHand("left");
        if (rightHandLandmarks != null) SolveHand("right");
    }

    // called from HolisticTrackingSolution.cs
    public void SetLeftHandLandmarks(NormalizedLandmarkList leftHandLandmarks)
    {
        this.leftHandLandmarks = leftHandLandmarks;
    }

    public void SetRightHandLandmarks(NormalizedLandmarkList rightHandLandmarks)
    {
        this.rightHandLandmarks = rightHandLandmarks;
    }

    private void SolveHand(string hand)
    {
        /*
        naming: 
            Lm denotes Landmark
            Tf denotes Transform
            v_ denotes vector
        */
        Transform[] fingerRootTfs = (hand.Equals("left")) ? rFingerRootTfs : lFingerRootTfs;
        Quaternion[,] prevRots = (hand.Equals("left")) ? prevRightRots : prevLeftRots;
        NormalizedLandmarkList handLandmarks = (hand.Equals("left")) ? leftHandLandmarks : rightHandLandmarks;

        Vector3 indexLm = new Vector3(-handLandmarks.Landmark[INDEX].X * WIDTH, -handLandmarks.Landmark[INDEX].Y * HEIGHT, handLandmarks.Landmark[INDEX].Z * WIDTH);
        Vector3 pinkyLm = new Vector3(-handLandmarks.Landmark[PINKY].X * WIDTH, -handLandmarks.Landmark[PINKY].Y * HEIGHT, handLandmarks.Landmark[PINKY].Z * WIDTH);
        if (!mirrorMode)
        {
            indexLm.z *= -1;
            pinkyLm.z *= -1;
        }

        Vector3 v_handLm = (indexLm - pinkyLm).normalized;
        Vector3 v_handTf = (fingerRootTfs[1].position - fingerRootTfs[4].position).normalized;
        Quaternion rot = Quaternion.FromToRotation(v_handLm, v_handTf);

        // convert Landmarks to Vector3s, with hand aligned with avatar
        Vector3[] landmarks = new Vector3[handLandmarks.Landmark.Count];
        for (int i = 0; i < landmarks.Length; i++)
        {
            /*
                landmark coordinate frame
                x-axis: left to right
                y-axis: bottom to top
                z-axis: into the screen
            */
            landmarks[i] = new Vector3(-handLandmarks.Landmark[i].X * WIDTH, -handLandmarks.Landmark[i].Y * HEIGHT, handLandmarks.Landmark[i].Z * WIDTH);
            if (!mirrorMode) landmarks[i].z *= -1;

            landmarks[i] = rot * landmarks[i];
        }        

        // solve rotations for each finger
        for (int i = 0; i < fingerRootTfs.Length; i++)
        {
            Transform parentTf = fingerRootTfs[i];
            int parentLmIdx = landmarkIdxs[i];

            // solve rotations for each joint
            for (int j = 0; j < JointsPerFinger; j++)
            {
                Transform childTf = parentTf.GetChild(0);
                int childLmIdx = parentLmIdx + 1;

                // current avatar phalange direction, in joint Transform's local space
                Vector3 vOld = (childTf.position - parentTf.position).normalized;
                vOld = parentTf.InverseTransformDirection(vOld);

                // current player phalange direction, in joint Transform's local space
                Vector3 vNew = (landmarks[childLmIdx] - landmarks[parentLmIdx]).normalized;
                vNew = parentTf.InverseTransformDirection(vNew);

                // smooth, interpolated rotation
                Quaternion rotOld = prevRots[i, j];
                Quaternion rotNew = parentTf.localRotation * Quaternion.FromToRotation(vOld, vNew).normalized;
                Quaternion rotSmooth = Quaternion.Slerp(rotOld, rotNew, SMOOTHING);

                // rotation constraint
                Vector3 angles = rotSmooth.eulerAngles;
                if (i == 0) // thumb
                {
                    if (j == 1 || j == 2)
                    {
                        angles.x = 0;
                        angles.y = 0;

                        while (angles.z > 180) angles.z -= 360;
                        while (angles.z < -180) angles.z += 360;

                        if (mirrorMode) angles.z = (hand.Equals("left")) ? Math.Clamp(angles.z, -90, 0) : Math.Clamp(angles.z, 0, 90);
                        else angles.z = (hand.Equals("right")) ? Math.Clamp(angles.z, -90, 0) : Math.Clamp(angles.z, 0, 90);

                        parentTf.localRotation = Quaternion.Euler(angles.x, angles.y, angles.z);
                    }
                }
                else // index, middle, ring, pinky
                {
                    while (angles.x > 180) angles.x -= 360;
                    while (angles.x < -180) angles.x += 360;
                    angles.x = Math.Clamp(angles.x, 0, 90);

                    angles.y = 0;

                    if (j == 0)
                    {
                        while (angles.z > 180) angles.z -= 360;
                        while (angles.z < -180) angles.z += 360;
                        angles.z = Math.Clamp(angles.z, -10, 10);
                    }
                    else
                    {
                        angles.z = 0;
                    }

                    parentTf.localRotation = Quaternion.Euler(angles.x, angles.y, angles.z);
                }

                prevRots[i, j] = parentTf.localRotation;
                
                parentTf = childTf;
                parentLmIdx++;
            }
        }
    }
}
