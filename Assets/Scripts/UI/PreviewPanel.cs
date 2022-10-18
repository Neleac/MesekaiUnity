using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PreviewPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject webcamView;
    private RectTransform panel;

    private Vector3 camShowPos;
    private Vector3 camHidePos;
    private Vector3 camGoalPos;
    private Vector2 panelShowPos;
    private Vector2 panelHidePos;
    private Vector2 panelGoalPos;

    [SerializeField] private float animTime;
    private float camAnimSpeed;
    private float panelAnimSpeed;
    
    void Start()
    {
        panel = GetComponent<RectTransform>();

        camShowPos = camera.transform.position;
        camHidePos = new Vector3(30, camShowPos.y, camShowPos.z);
        camGoalPos = camera.transform.position;
        camAnimSpeed = Vector3.Distance(camShowPos, camHidePos) / animTime;
        
        panelShowPos = panel.anchoredPosition;
        panelHidePos = new Vector2(-1920f, panelShowPos.y);
        panelGoalPos = panel.anchoredPosition;
        panelAnimSpeed = Vector3.Distance(panelShowPos, panelHidePos) / animTime;
    }

    void Update()
    {
        Vector3 camPos = camera.transform.position;
        if (camPos != camGoalPos)
        {
            camera.transform.position = Vector3.MoveTowards(camPos, camGoalPos, camAnimSpeed * Time.deltaTime);
        }

        Vector2 panelPos = panel.anchoredPosition;
        if (panelPos != panelGoalPos)
        {
            panel.anchoredPosition = Vector2.MoveTowards(panelPos, panelGoalPos, panelAnimSpeed * Time.deltaTime);
        }

    }

    public void HideShow()
    {
        if (panel.anchoredPosition == panelShowPos)
        {
            camGoalPos = camHidePos;
            panelGoalPos = panelHidePos;
            buttonText.text = "Show Panel";
            webcamView.SetActive(false);
        }
        else
        {
            camGoalPos = camShowPos;
            panelGoalPos = panelShowPos;
            buttonText.text = "Hide Panel";
            webcamView.SetActive(true);
        }
    }
}
