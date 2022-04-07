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
    
    // landmark indices
    const int NOSE = 4;        // tip of nose
    const int NASAL = 5;       // 1 point above nose
    const int LEFT = 454;      // left most point
    const int RIGHT = 234;     // right most point
    const int TOP = 10;        // top most point                       
    const int BOT = 152;       // bottom most point

    const float SMOOTHING = 0.1f;   // lower value = smoother, but less responsive

    [SerializeField] private Transform headBone;
    [SerializeField] private SkinnedMeshRenderer faceMesh;
    [SerializeField] private bool mirrorMode;
    [SerializeField] private bool emotionDetection;

    private NormalizedLandmarkList faceLandmarks;

    void Start()
    {
        faceLandmarks = null;
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
        ///////////////////////
        // PROCESS LANDMARKS //
        ///////////////////////

        Vector3 nose = new Vector3(-faceLandmarks.Landmark[NOSE].X * WIDTH, -faceLandmarks.Landmark[NOSE].Y * HEIGHT, faceLandmarks.Landmark[NOSE].Z * WIDTH);
        
        // convert Landmarks to Vector3s, with nose being the origin
        Vector3[] landmarks = new Vector3[faceLandmarks.Landmark.Count];
        for (int i = 0; i < landmarks.Length; i++)
        {
            /*
            landmark coordinate frame
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
        face plane coordinate frame
            x-axis: right to left
            y-axis: bottom to top
            z-axis: out of the screen
        */
        Vector3 faceY = (nasal - origin).normalized;
        Vector3 faceZ = normal;
        Vector3 faceX = Vector3.Cross(faceY, faceZ).normalized;
        Matrix4x4 faceBasis = new Matrix4x4(faceX, faceY, faceZ, new Vector4(0, 0, 0, 1));
        
        /////////////////
        // ROTATE HEAD //
        /////////////////

        // avatar head local axes
        Vector3 avatarX = headBone.parent.worldToLocalMatrix.MultiplyVector(headBone.right).normalized;
        Vector3 avatarY = headBone.parent.worldToLocalMatrix.MultiplyVector(headBone.up).normalized;
        Vector3 avatarZ = headBone.parent.worldToLocalMatrix.MultiplyVector(headBone.forward).normalized;

        // rotate avatar head
        double xRad = Math.Acos(faceZ.y) - Math.PI / 2;
        float xDeg = (float)(180 / Math.PI * xRad) - 30;

        double yRad = Math.Acos(faceZ.x) - Math.PI / 2;
        float yDeg = (float)(180 / Math.PI * yRad);
        if (!mirrorMode) yDeg *= -1;

        double zRad = Math.PI / 2 - Math.Acos(faceY.x);
        float zDeg = (float)(180 / Math.PI * zRad);
        if (!mirrorMode) zDeg *= -1;

        Quaternion oldRot = headBone.localRotation;
        Quaternion newRot = Quaternion.Euler(xDeg, yDeg, zDeg);
        headBone.localRotation = Quaternion.Slerp(oldRot, newRot, SMOOTHING);

        ///////////////////////////
        // CALCULATE BLENDSHAPES //
        ///////////////////////////

        // convert landmarks to 2D
        for (int i = 0; i < landmarks.Length; i++)
        {
            // project landmarks onto face plane
            landmarks[i] = Vector3.ProjectOnPlane(landmarks[i], normal);

            // change of basis: landmark axes -> face plane axes
            // landmark = (x, y, 0)
            landmarks[i] = faceBasis.inverse.MultiplyVector(landmarks[i]);

            // normalize by head dimensions
            landmarks[i].x /= headWidth;
            landmarks[i].y /= headHeight;
        }

        // eyes
        Vector3 eyeRT = landmarks[27];
        Vector3 eyeRB = landmarks[23];
        Vector3 eyeLT = landmarks[257];
        Vector3 eyeLB = landmarks[253];
        if (!mirrorMode)
        {
            (eyeRT, eyeLT) = (eyeLT, eyeRT);
            (eyeRB, eyeLB) = (eyeLB, eyeRB);
        }

        SetBlendshape("eyeBlinkLeft", eyeRT.y - eyeRB.y, 0.1f, 0.08f);
        SetBlendshape("eyeBlinkRight", eyeLT.y - eyeLB.y, 0.1f, 0.08f);

        // SetBlendshape("eyeSquintLeft", eyeRT.y - eyeRB.y, 0.1f, 0.095f);
        // SetBlendshape("eyeSquintRight", eyeLT.y - eyeLB.y, 0.1f, 0.095f);

        SetBlendshape("eyeWideLeft", eyeRT.y - eyeRB.y, 0.1f, 0.12f);
        SetBlendshape("eyeWideRight", eyeLT.y - eyeLB.y, 0.1f, 0.12f);

        // eyebrows
        Vector3 browR = landmarks[66];
        Vector3 browL = landmarks[296];
        if (!mirrorMode) (browR, browL) = (browL, browR);

        // SetBlendshape("browOuterUpLeft", browR.y, 0.35f, 0.4f);
        // SetBlendshape("browOuterUpRight", browL.y, 0.35f, 0.4f);
        SetBlendshape("browInnerUp", 0.5f * (browR.y + browL.y), 0.35f, 0.4f);

        // SetBlendshape("browDownLeft", browR.y, 0.35f, 0.33f);
        // SetBlendshape("browDownRight", browL.y, 0.35f, 0.33f);

        // mouth
        Vector3 mouthT = landmarks[13];
        Vector3 mouthB = landmarks[14];
        Vector3 mouthL = landmarks[291];
        Vector3 mouthR = landmarks[61];
        if (!mirrorMode) (mouthR, mouthL) = (mouthL, mouthR);

        SetBlendshape("jawOpen", mouthT.y - mouthB.y, 0.01f, 0.20f);

        SetBlendshape("mouthSmileLeft", mouthR.y, -0.22f, -0.2f);
        SetBlendshape("mouthSmileRight", mouthL.y, -0.22f, -0.2f);

        SetBlendshape("mouthFrownLeft", mouthR.y, -0.22f, -0.30f);
        SetBlendshape("mouthFrownRight", mouthL.y, -0.22f, -0.30f);

        // nose
        // Vector3 noseR = landmarks[64];
        // Vector3 noseL = landmarks[294];
        // SetBlendshape("noseSneerLeft", noseR.y, -0.027f, -0.022f);
        // SetBlendshape("noseSneerRight", noseL.y, -0.027f, -0.022f);

        if (emotionDetection)
        {
            // bool oShapeMouth = Math.Abs(mouthT.y - mouthB.y) / Math.Abs(mouthR.x - mouthL.x) > 0.18;
            // bool vShapeMouth = !oShapeMouth && Math.Abs((mouthR.y + mouthL.y) / 2 - mouthB.y) / Math.Abs(mouthR.x - mouthL.x) > 0.08;
            // print(Math.Abs(mouthT.y - mouthB.y) / Math.Abs(mouthR.x - mouthL.x) + "," + Math.Abs((mouthR.y + mouthL.y) / 2 - mouthB.y) / Math.Abs(mouthR.x - mouthL.x));

            // TODO: factor in eyes

            int index = faceMesh.sharedMesh.GetBlendShapeIndex("mouthSmileLeft");
            float smileLeft = faceMesh.GetBlendShapeWeight(index);
            index = faceMesh.sharedMesh.GetBlendShapeIndex("mouthSmileRight");
            float smileRight = faceMesh.GetBlendShapeWeight(index);
            bool happy = smileLeft > 25 && smileRight > 25;

            index = faceMesh.sharedMesh.GetBlendShapeIndex("mouthFrownLeft");
            float frownLeft = faceMesh.GetBlendShapeWeight(index);
            index = faceMesh.sharedMesh.GetBlendShapeIndex("mouthFrownRight");
            float frownRight = faceMesh.GetBlendShapeWeight(index);
            bool sad = frownLeft > 60 && frownRight > 60;

            index = faceMesh.sharedMesh.GetBlendShapeIndex("jawOpen");
            float jawOpen = faceMesh.GetBlendShapeWeight(index);
            bool wow = jawOpen > 75;

            string emotion = "Neutral";
            if (wow) emotion = "Wow";
            else if (happy) emotion = "Happy";
            else if (sad) emotion = "Sad";
            
            GameObject obj = GameObject.Find("Emotion");
            obj.GetComponent<TMPro.TextMeshProUGUI>().text = emotion;
        }
    }

    private void SetBlendshape(string name, float value, float low, float high)
    {   
        // linear interpolate
        float newWeight = (value - low) / (high - low);

        // clamp between [0, 100]
        newWeight = Math.Clamp(newWeight, 0, 1) * 100;

        // interpolate with previous weight for smoother animation
        int index = faceMesh.sharedMesh.GetBlendShapeIndex(name);
        float oldWeight = faceMesh.GetBlendShapeWeight(index);
        faceMesh.SetBlendShapeWeight(index, (1 - SMOOTHING) * oldWeight + SMOOTHING * newWeight);
    }
}
