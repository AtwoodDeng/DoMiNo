using UnityEngine;
using System.Collections;

public class FixedObject : Unit {

	public override void Confirm ()
	{
		base.Confirm ();
		gameObject.layer = LayerMask.NameToLayer ("Floor");
		if (m_rigidbody != null)
			m_rigidbody.isKinematic = true;
	}
}
