using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class Footsteps : MonoBehaviour
{
    [SerializeField] private StarterAssetsInputs input;
    
    const float deltaTimeWalk = 0.6f;
    const float deltaTimeRun = 0.3f;
    float timer;

    void Start()
    {
        timer = 0;
    }

    void FixedUpdate()
    {
        if (input.sprint)
        {
            // RUNNING

            timer += Time.fixedDeltaTime;

            RaycastHit hit;
            float maxDist = 1.0f;
            int layerMask = 1 << 0; // layer 0: Default
            
            if (timer >= deltaTimeRun && Physics.Raycast(transform.position, -Vector3.up, out hit, maxDist, layerMask))
            {    
                transform.Find(hit.collider.tag).GetComponent<FMODUnity.StudioEventEmitter>().Play();
                timer = 0;
            }
        }
        else if (input.move.magnitude > 0)
        {
            // WALKING

            timer += Time.fixedDeltaTime;

            RaycastHit hit;
            float maxDist = 1.0f;
            int layerMask = 1 << 0; // layer 3: Default
            
            if (timer >= deltaTimeWalk && Physics.Raycast(transform.position, -Vector3.up, out hit, maxDist, layerMask))
            {    
                transform.Find(hit.collider.tag).GetComponent<FMODUnity.StudioEventEmitter>().Play();
                timer = 0;
            }
        }
        else
        {
            timer = 0;
        }
    }
}
