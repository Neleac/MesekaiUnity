using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene.Animation
    {
    public class StandingBehaviour : StateMachineBehaviour
        {
        private float lastPoseChangeDuration = float.PositiveInfinity;
        private float poseChangeFrequency = 2f;
        private float startPose;
        private float endPose;
        private int poseParaHash;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
            poseParaHash = Animator.StringToHash("Pose");
            base.OnStateEnter(animator, stateInfo, layerIndex);
            }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {

            lastPoseChangeDuration += Time.deltaTime;

            if (lastPoseChangeDuration >= poseChangeFrequency)
                {
                startPose = animator.GetFloat(poseParaHash);
                endPose = Random.Range(0f, 1f);
                lastPoseChangeDuration = 0;
                }

            animator.SetFloat(poseParaHash, Mathf.Lerp(startPose, endPose, lastPoseChangeDuration / poseChangeFrequency));

            base.OnStateUpdate(animator, stateInfo, layerIndex);
            }

        }
    }