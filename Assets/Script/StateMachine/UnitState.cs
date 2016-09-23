using UnityEngine;
using System.Collections;

public class UnitState : MStateMachineBehavior {

	protected Unit parentUnit;

	public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter (animator, stateInfo, layerIndex);
		parentUnit = animator.gameObject.GetComponent<Unit> ();
	}
}
