using UnityEngine;
using System.Collections;

public class FixedObject : EditableUnit {

	public override void Settle ()
	{
		base.Settle ();
		gameObject.layer = LayerMask.NameToLayer ("Floor");
		if (m_rigidbody != null)
			m_rigidbody.isKinematic = true;
	}
}
