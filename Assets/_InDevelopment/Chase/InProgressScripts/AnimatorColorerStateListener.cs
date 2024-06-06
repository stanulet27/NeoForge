using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NeoForge.UITools
{
    public class AnimatorColorerStateListener : StateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.TryGetComponent<ButtonColorer>(out var controller))
            {
                //controller.OnAnimatorStateChange(DetermineWhichState(stateInfo));
            }
        }
        
        
    }
}