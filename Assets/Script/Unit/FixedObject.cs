using UnityEngine;
using System.Collections;

public class FixedObject : EditableUnit {

	public override void Confirm ()
	{
		base.Confirm ();
		gameObject.layer = LayerMask.NameToLayer ("Floor");
		if (m_rigidbody != null)
			m_rigidbody.isKinematic = true;
	}

	protected override void EnterSettled ()
	{
		base.EnterSettled ();
		gameObject.layer = LayerMask.NameToLayer ("Floor");
		if (m_rigidbody != null)
			m_rigidbody.isKinematic = true;
	}
}
