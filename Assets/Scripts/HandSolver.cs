using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;

public class HandSolver : MonoBehaviour
{
    private NormalizedLandmarkList leftHandLandmarks;
    private NormalizedLandmarkList rightHandLandmarks;

    void Start()
    {
        leftHandLandmarks = null;
        rightHandLandmarks = null;
    }

    void Update()
    {
        if (leftHandLandmarks != null) SolveHand("left");
        if (rightHandLandmarks != null) SolveHand("right");

        //if (leftHandLandmarks != null) RockPaperSscissors();
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
        
    }

    private void RockPaperSscissors()
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
