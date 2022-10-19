using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FullBodyView : MonoBehaviour
{
    public GameObject avatar;
    [SerializeField] private float animTime;
    [SerializeField] private TextMeshProUGUI buttonText;

    private Vector3 halfPos, fullPos, goalPos;
    private Vector3 halfScl, fullScl, goalScl;
    private float posAnimSpeed, sclAnimSpeed;
    
    void Start()
    {
        halfPos = avatar.transform.position;
        halfScl = avatar.transform.localScale;

        fullPos = new Vector3(40, -45, 90);
        fullScl = new Vector3(50, 50, 50);

        goalPos = halfPos;
        goalScl = halfScl;

        posAnimSpeed = Vector3.Distance(halfPos, fullPos) / animTime;
        sclAnimSpeed = Vector3.Distance(halfScl, fullScl) / animTime;
    }

    void Update()
    {
        Vector3 avatarPos = avatar.transform.position;
        Vector3 avatarScl = avatar.transform.localScale;
        if (avatarPos != goalPos)
        {
            avatar.transform.position = Vector3.MoveTowards(avatarPos, goalPos, posAnimSpeed * Time.deltaTime);
            avatar.transform.localScale = Vector3.MoveTowards(avatarScl, goalScl, sclAnimSpeed * Time.deltaTime);
        }
    }

    public void fullBodyTracking()
    {
        if (buttonText.text == "Full Body")
        {
            goalPos = fullPos;
            goalScl = fullScl;
            buttonText.text = "Half Body";
        }
        else
        {
            goalPos = halfPos;
            goalScl = halfScl;
            buttonText.text = "Full Body";
        }
    }
}
