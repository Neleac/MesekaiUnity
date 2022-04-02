using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;

public class FaceSolver : MonoBehaviour
{
    // TODO: replace magic numbers with input dimensions from Start/Image Source/Web Cam Source
    const int WIDTH = 640;
    const int HEIGHT = 480;
    
    // face mesh indices
    const int NOSE = 4;        // tip of nose
    const int NASAL = 5;       // 1 point above nose
    const int LEFT = 454;      // left most point
    const int RIGHT = 234;     // right most point
    const int TOP = 10;        // top most point                       
    const int BOT = 152;       // bottom most point

    const float SMOOTHING = 0.1f;   // lower value = smoother, but less responsive

    [SerializeField] private Transform headBone;
    private NormalizedLandmarkList faceLandmarks;

    void Start()
    {
        faceLandmarks = null;

        Debug.Log(headBone.eulerAngles.x);
    }

    void Update()
    {
        if (faceLandmarks != null) SolveFace();
    }

    // called from HolisticTrackingSolution.cs
    public void SetFaceLandmarks(NormalizedLandmarkList faceLandmarks)
    {
        this.faceLandmarks = faceLandmarks;
    }

    private void SolveFace()
    {
        // convert Landmarks to Vector3s, with nose being the origin
        Vector3 nose = new Vector3(-faceLandmarks.Landmark[NOSE].X * WIDTH, -faceLandmarks.Landmark[NOSE].Y * HEIGHT, faceLandmarks.Landmark[NOSE].Z * WIDTH);

        Vector3[] landmarks = new Vector3[faceLandmarks.Landmark.Count];
        for (int i = 0; i < landmarks.Length; i++)
        {
            /*
            image coordinate frame
                x-axis: left to right
                y-axis: bottom to top
                z-axis: into the screen
            */
            landmarks[i] = new Vector3(-faceLandmarks.Landmark[i].X * WIDTH, -faceLandmarks.Landmark[i].Y * HEIGHT, faceLandmarks.Landmark[i].Z * WIDTH) - nose;
        }

        // center of head
        Vector3 headLeft = landmarks[LEFT];
        Vector3 headRight = landmarks[RIGHT];
        Vector3 headMid = Vector3.Lerp(headLeft, headRight, 0.5f);
        
        // width, height of head
        Vector3 headTop = landmarks[TOP];
        Vector3 headBot = landmarks[BOT];
        float headWidth = Vector3.Distance(headRight, headLeft);
        float headHeight = Vector3.Distance(headTop, headBot);

        // face plane origin (nose), normal (z-axis), local axes
        Vector3 origin = landmarks[NOSE];
        Vector3 normal = (origin - headMid).normalized;
        Vector3 nasal = Vector3.ProjectOnPlane(landmarks[NASAL], normal);

        /*
        face coordinate frame
            x-axis: right to left
            y-axis: bottom to top
            z-axis: out of the screen
        */
        Vector3 faceY = (nasal - origin).normalized;
        Vector3 faceZ = normal;
        Vector3 faceX = Vector3.Cross(faceY, faceZ).normalized;
        
        // avatar head local axes
        Vector3 avatarX = headBone.parent.worldToLocalMatrix.MultiplyVector(headBone.right).normalized;
        Vector3 avatarY = headBone.parent.worldToLocalMatrix.MultiplyVector(headBone.up).normalized;
        Vector3 avatarZ = headBone.parent.worldToLocalMatrix.MultiplyVector(headBone.forward).normalized;

        // rotate avatar head
        double xRad = Math.Acos(faceZ.y) - Math.PI / 2;
        float xDeg = (float)(180 / Math.PI * xRad) - 30;

        double yRad = Math.Acos(faceZ.x) - Math.PI / 2;
        float yDeg = (float)(180 / Math.PI * yRad);

        double zRad = Math.PI / 2 - Math.Acos(faceY.x);
        float zDeg = (float)(180 / Math.PI * zRad);

        Quaternion currRot = headBone.localRotation;
        Quaternion newRot = Quaternion.Euler(xDeg, yDeg, zDeg);
        headBone.localRotation = Quaternion.Slerp(currRot, newRot, SMOOTHING);
    }
}
