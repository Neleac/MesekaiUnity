using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class Footsteps : MonoBehaviour
{
    [SerializeField] private StarterAssetsInputs input;
    [SerializeField] private ThirdPersonController controller;
    
    const float deltaTimeWalk = 0.6f;
    const float deltaTimeRun = 0.4f;
    float timer;
    bool jumped;

    void Start()
    {
        timer = 0;
        jumped = false;
    }

    void FixedUpdate()
    {
        // Jumping
        if (!controller.Grounded) 
        {
            jumped = true;
        }
        else if (jumped && controller.Grounded)
        {
            jumped = false;

            RaycastHit hit;
            float maxDist = 1.0f;
            int layerMask = 1 << 0; // layer 0: Default
            
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, maxDist, layerMask))
            {    
                transform.Find(hit.collider.tag + " Jump").GetComponent<FMODUnity.StudioEventEmitter>().Play();
            }
        }
        // Walking and Running
        else if (input.move.magnitude > 0)
        {
            float deltaTime = (input.sprint) ? deltaTimeRun : deltaTimeWalk;

            timer += Time.fixedDeltaTime;

            RaycastHit hit;
            float maxDist = 1.0f;
            int layerMask = 1 << 0; // layer 0: Default
            
            if (timer >= deltaTime && Physics.Raycast(transform.position, -Vector3.up, out hit, maxDist, layerMask))
            {    
                transform.Find(hit.collider.tag + " Move").GetComponent<FMODUnity.StudioEventEmitter>().Play();
                timer = 0;
            }
        }
        else
        {
            timer = 0;
        }
    }
}
