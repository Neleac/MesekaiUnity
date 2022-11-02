using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTransfer : MonoBehaviour
{
    public Animator src, tgt;

    void Update()
    {
        foreach(AnimatorControllerParameter param in src.parameters)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Float:
                    tgt.SetFloat(param.name, src.GetFloat(param.name));
                    break;
                case AnimatorControllerParameterType.Int:
                    tgt.SetInteger(param.name, src.GetInteger(param.name));
                    break;
                case AnimatorControllerParameterType.Bool:
                    tgt.SetBool(param.name, src.GetBool(param.name));
                    break;
                default:
                    break;
            }
        }
    }

    public void SetController()
    {
        tgt.runtimeAnimatorController = src.runtimeAnimatorController;
    }
}
