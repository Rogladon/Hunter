using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hunter {
	public class SuperSlash : StateMachineBehaviour {
		public float speed = 1f;
		public Entity entity;
		//OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			animator.SetBool("superAttack", true);
			entity = animator.GetComponentInParent<Entity>();
			entity.AddBuff(entity.weapon.superSlash);
			animator.SetInteger("attack", 0); 
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			entity.transform.position += entity.direction * speed * Time.deltaTime;
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			animator.SetBool("superAttack", false);
			animator.SetInteger("attack", 0);
			entity.RemoveBuff(entity.weapon.superSlash);
		}

		// OnStateMove is called right after Animator.OnAnimatorMove()
		//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		//{
		//    // Implement code that processes and affects root motion
		//}

		// OnStateIK is called right after Animator.OnAnimatorIK()
		//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		//{
		//    // Implement code that sets up animation IK (inverse kinematics)
		//}
	}
}
