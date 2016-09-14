using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	public enum State
	{
		None,
		Editting,
		Settled,
		Running,
	}
	public State state;

	private static Unit m_edittingUnit;
	public static Unit EdittingUnit{
		get { return m_edittingUnit; }
	}

	public Collider m_collider;
	public Rigidbody m_rigidbody;

	[SerializeField] Vector3 initOffset;

	virtual public void OnEnable()
	{
		M_Event.EditCancle += M_Event_EditCancle;
	}

	void M_Event_EditCancle (MsgArg arg)
	{
		Debug.Log ("Unit Edit Cancle");
		if (state == State.Editting && EdittingUnit == this) {
			GameObject.Destroy (gameObject);
		}
	}

	virtual public void OnDisable()
	{
		M_Event.EditCancle -= M_Event_EditCancle;
	}

	void Awake()
	{
		if (m_collider == null)
			m_collider = GetComponent<Collider> ();
		if (m_rigidbody == null)
			m_rigidbody = GetComponent<Rigidbody> ();
	}

	public virtual void StartEditting()
	{
		state = State.Editting;
		m_edittingUnit = this;
		if (m_collider != null)
			m_collider.isTrigger = true;
		if (m_rigidbody != null)
			m_rigidbody.isKinematic = true;
		transform.SetParent (Global.world);
	}


	public virtual void Confirm ()
	{
		state = State.Settled;

		if (m_collider != null)
			m_collider.isTrigger = false;
		if (m_rigidbody != null) {
			m_rigidbody.isKinematic = false;
			m_rigidbody.velocity = Vector3.zero;
		}
	}

	void Update()
	{
		switch (state) {
		case State.Editting:
				
			Vector2 screenPosition = InputManager.Instance.GetScreenPosition ();
			Debug.Log ("Screen Position" + screenPosition);
			if ( screenPosition.Equals (Global.Vector2Infinity) || EdittingUnit != this )
				transform.position = Global.Vector3Infinity;
			else {
				Ray ray = Camera.main.ScreenPointToRay (screenPosition);
				RaycastHit hitInfo;
				if (Physics.Raycast (ray, out hitInfo, Mathf.Infinity, Global.StandableMask)) {
					transform.position = hitInfo.point + initOffset;
				} else {
					transform.position = Global.Vector3Infinity;
				}
			}
			break;
		default:
			break;
		}


	}


}
