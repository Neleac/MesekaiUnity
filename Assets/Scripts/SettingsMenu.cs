using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mediapipe.Unity.Holistic;

public class SettingsMenu : MonoBehaviour
{
    private GameObject mocapButton;
    [SerializeField] private GameObject playerAvatar;
    [SerializeField] private GameObject mediapipeSolution;

    void Start()
    {
        mocapButton = transform.Find("Motion Controls").Find("Toggle").gameObject;
    }

    void Update()
    {
        
    }

    public void MotionControls()
    {
        Toggle button = mocapButton.GetComponent<Toggle>();

        playerAvatar.GetComponent<PoseSolver>().enabled = button.isOn;
        playerAvatar.GetComponent<HandSolver>().enabled = button.isOn;
        playerAvatar.GetComponent<FaceSolver>().enabled = button.isOn;
        playerAvatar.GetComponent<MotionToggle>().enabled = button.isOn;
    }

    public void MotionQuality(string level)
    {
        HolisticTrackingGraph graph = mediapipeSolution.GetComponent<HolisticTrackingGraph>();
        
        switch (level)
        {
            case "low":
                graph.modelComplexity = HolisticTrackingGraph.ModelComplexity.Lite;
                break;
            case "medium":
                graph.modelComplexity = HolisticTrackingGraph.ModelComplexity.Full;
                break;
            case "high":
                graph.modelComplexity = HolisticTrackingGraph.ModelComplexity.Heavy;
                break;
            default:
                break;
        }
    }
}
