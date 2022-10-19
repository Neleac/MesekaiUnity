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

    private Vector3 camShowPos, camHidePos, camGoalPos;
    private Vector2 panelShowPos, panelHidePos, panelGoalPos;

    [SerializeField] private float animTime;
    private float camAnimSpeed, panelAnimSpeed;
    
    void Start()
    {
        panel = GetComponent<RectTransform>();

        camShowPos = camera.transform.position;
        camHidePos = new Vector3(30, camShowPos.y, camShowPos.z);
        camGoalPos = camShowPos;
        camAnimSpeed = Vector3.Distance(camShowPos, camHidePos) / animTime;
        
        panelShowPos = panel.anchoredPosition;
        panelHidePos = new Vector2(-1920f, panelShowPos.y);
        panelGoalPos = panelShowPos;
        panelAnimSpeed = Vector3.Distance(panelShowPos, panelHidePos) / animTime;
    }

    void Update()
    {
        Vector3 camPos = camera.transform.position;
        Vector2 panelPos = panel.anchoredPosition;
        if (camPos != camGoalPos)
        {
            camera.transform.position = Vector3.MoveTowards(camPos, camGoalPos, camAnimSpeed * Time.deltaTime);
            panel.anchoredPosition = Vector2.MoveTowards(panelPos, panelGoalPos, panelAnimSpeed * Time.deltaTime);
        }
    }

    public void HideShow()
    {
        if (buttonText.text == "Hide Panel")
        {
            buttonText.text = "Show Panel";
            webcamView.SetActive(false);
            
            camGoalPos = camHidePos;
            panelGoalPos = panelHidePos;
        }
        else
        {
            buttonText.text = "Hide Panel";
            webcamView.SetActive(true);

            camGoalPos = camShowPos;
            panelGoalPos = panelShowPos;
        }
    }
}
