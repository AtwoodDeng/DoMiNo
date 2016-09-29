using UnityEngine;
using System.Collections;

public class EditableUnit : Unit
{

	public enum State
	{
		None,
		/// <summary>
		/// find a place to place the unit
		/// define the position of the unit
		/// </summary>
		Place,
		/// <summary>
		/// Turn the unit
		/// define the rotation of the unit
		/// </summary>
		Turn,
		/// <summary>
		/// The unit has been settled on the world
		/// </summary>
		Settle,
		/// <summary>
		/// The unit is under modifying
		/// </summary>
		Modify,
		/// <summary>
		/// The unit is runing in the world 
		/// </summary>
		Run,

		/// <summary>
		/// Being deleted
		/// </summary>
		Deleted,
	}

	public State state {
		get {
			return m_stateMachine.State;
		}
	}

	public AStateMachine<State> m_stateMachine;

	private static Unit m_edittingUnit;

	public static Unit EdittingUnit {
		get { return m_edittingUnit; }
	}

	bool isEditting {
		get {
			return (state == State.Turn || state == State.Place) && (EdittingUnit == this);
		}
	}

	[System.Serializable]
	public struct Sensity
	{
		[Range (0, 2)]
		public float rotationSensity;
	}

	[SerializeField] Sensity sensity;


	public Collider m_collider;
	public Rigidbody m_rigidbody;

	bool onFocus {
		get { return !(transform.position.Equals (Global.Vector3Infinity)); }
	}

	[SerializeField] Vector3 initOffset;

	virtual public void OnEnable ()
	{
//		M_Event.editorEvents [(int)MEditorEvent.EditCancle] += M_Event_EditCancle;
//		M_Event.editorEvents [(int)MEditorEvent.StartRunning] += M_Event_StartRunning;

		for (int i = 0; i < System.Enum.GetNames (typeof(MInputType)).Length; ++i) {
			M_Event.inputEvents [i] += OnInput;
		}


		for (int i = 0; i < System.Enum.GetNames (typeof(MEditorEvent)).Length; ++i) {
			M_Event.editorEvents [i] += OnEditorEvent;
		}
	}

	public delegate void InputFeedback (InputArg arg);

	public InputFeedback[] inputFeedback = new InputFeedback[System.Enum.GetNames (typeof(MInputType)).Length];

	public delegate void EditorEventFeedback (EditorArg arg);

	public EditorEventFeedback[] editorEventFeedback = new EditorEventFeedback[System.Enum.GetNames (typeof(MEditorEvent)).Length];


	void OnInput (InputArg arg)
	{
		if (inputFeedback != null && inputFeedback.Length > ((int)arg.type)) {
			if (inputFeedback [(int)arg.type] != null)
				inputFeedback [(int)arg.type] (arg);
		}
	}

	void OnEditorEvent( EditorArg arg )
	{
		if (editorEventFeedback != null && editorEventFeedback.Length > ((int)arg.type)) {
			if (editorEventFeedback [(int)arg.type] != null)
				editorEventFeedback [(int)arg.type] (arg);
		}
	}

	/// ===================  Start State Machine  ========================
	/// <summary>
	/// Initilize the state machine.
	/// </summary>

	virtual public void InitStateMachine ()
	{
		m_stateMachine = new AStateMachine<State> ();
		m_stateMachine.State = State.None;
		m_stateMachine.AddEnter (State.Place, EnterPlacing);
		m_stateMachine.AddUpdate (State.Place, UpdatePlacing);
		m_stateMachine.AddExit (State.Place, ExitPlacing);

		m_stateMachine.AddEnter (State.Turn, EnterSpinning);
		m_stateMachine.AddUpdate (State.Turn, UpdateSpinning);
		m_stateMachine.AddExit (State.Turn, ExitSpinning);

		m_stateMachine.AddEnter (State.Settle, EnterSettled);
		m_stateMachine.AddUpdate (State.Settle, UpdateSettled);
		m_stateMachine.AddExit (State.Settle, ExitSettled);

		m_stateMachine.AddEnter (State.Run, EnterRun);
		m_stateMachine.AddUpdate (State.Run, UpdateRun);
		m_stateMachine.AddExit (State.Run, ExitRun);


		m_stateMachine.AddEnter (State.Modify, EnterModify);
		m_stateMachine.AddUpdate (State.Modify, UpdateModify);
		m_stateMachine.AddExit (State.Modify, ExitModify);
	}

	virtual protected void EnterPlacing ()
	{
		m_edittingUnit = this;
		if (m_collider != null)
			m_collider.isTrigger = true;
		if (m_rigidbody != null)
			m_rigidbody.isKinematic = true;
		transform.SetParent (Global.world);

		inputFeedback [(int)MInputType.MainButtonDown] = delegate(InputArg arg) {
			if (onFocus)
				m_stateMachine.State = State.Turn;
		};

		editorEventFeedback [(int)MEditorEvent.StartRunning] = delegate(EditorArg arg) {
			Delete();
		};
		editorEventFeedback [(int)MEditorEvent.EditCancle] = delegate(EditorArg arg) {
			Delete();
		};
	}

	virtual protected void UpdatePlacing ()
	{
		if (EdittingUnit == this) {
			Vector2 screenPosition;
			if (InputManager.Instance.GetScreenPos (out screenPosition)) {
				Ray ray = Camera.main.ScreenPointToRay (screenPosition);
				RaycastHit hitInfo;
				if (Physics.Raycast (ray, out hitInfo, Mathf.Infinity, Global.StandableMask)) {
					transform.position = hitInfo.point + initOffset;
				} else {
					transform.position = Global.Vector3Infinity;

				}
			} else {
				transform.position = Global.Vector3Infinity;
			}
		} else {
			transform.position = Global.Vector3Infinity;
		}
	}

	virtual protected void ExitPlacing ()
	{
		inputFeedback [(int)MInputType.MainButtonDown] = null;
		editorEventFeedback [(int)MEditorEvent.StartRunning] = null;
		editorEventFeedback [(int)MEditorEvent.EditCancle] = null;
	}

	virtual protected void EnterSpinning ()
	{
		inputFeedback [(int)MInputType.MainButtonUp] = delegate(InputArg arg) {
			m_stateMachine.State = State.Settle;
		};

		inputFeedback [(int)MInputType.Motion] = delegate (InputArg arg) {
			transform.Rotate (Vector3.up, -sensity.rotationSensity * arg.offset.x);	
		};


		editorEventFeedback [(int)MEditorEvent.StartRunning] = delegate(EditorArg arg) {
			Delete();
		};
		editorEventFeedback [(int)MEditorEvent.EditCancle] = delegate(EditorArg arg) {
			Delete();
		};
	}

	virtual protected void UpdateSpinning ()
	{

	}

	virtual protected void ExitSpinning ()
	{
		inputFeedback [(int)MInputType.MainButtonUp] = null;

		inputFeedback [(int)MInputType.Motion] = null;

		editorEventFeedback [(int)MEditorEvent.StartRunning] = null;
		editorEventFeedback [(int)MEditorEvent.EditCancle] = null;
	}

	virtual protected void EnterSettled ()
	{
		Settle ();

		editorEventFeedback [(int)MEditorEvent.StartRunning] = delegate(EditorArg arg) {
			m_stateMachine.State = State.Run;
		};
	}

	virtual protected void UpdateSettled ()
	{
	}

	virtual protected void ExitSettled ()
	{
		editorEventFeedback [(int)MEditorEvent.StartRunning] = null;
	}

	virtual protected void EnterRun ()
	{
		editorEventFeedback [(int)MEditorEvent.EndRunning] = delegate(EditorArg arg) {
			m_stateMachine.State = State.Settle;
		};
	}

	virtual protected void UpdateRun ()
	{
	}

	virtual protected void ExitRun ()
	{
		editorEventFeedback [(int)MEditorEvent.EndRunning] = null;
	}

	virtual protected void EnterModify ()
	{
	}

	virtual protected void UpdateModify ()
	{
	}

	virtual protected void ExitModify ()
	{

	}

	/////================== End State Machine ===============================

	void M_Event_StartRunning (MsgArg arg)
	{
	}

	void M_Event_EditCancle (MsgArg arg)
	{
	}

	virtual public void OnDisable ()
	{
//		M_Event.editorEvents [(int)MEditorEvent.StartRunning] -= M_Event_StartRunning;
//		M_Event.editorEvents [(int)MEditorEvent.EditCancle] -= M_Event_EditCancle;

		for (int i = 0; i < System.Enum.GetNames (typeof(MInputType)).Length; ++i) {
			M_Event.inputEvents [i] -= OnInput;
		}

		for (int i = 0; i < System.Enum.GetNames (typeof(MEditorEvent)).Length; ++i) {
			M_Event.editorEvents [i] -= OnEditorEvent;
		}
	}

	public virtual void StartEditting ()
	{
		//		state = State.Placing;
		m_stateMachine.State = State.Place;
		m_edittingUnit = this;
		if (m_collider != null)
			m_collider.isTrigger = true;
		if (m_rigidbody != null)
			m_rigidbody.isKinematic = true;
		transform.SetParent (Global.world);
	}

	override public void Init (UnitData data)
	{
		base.Init (data);
		m_stateMachine.State = State.Settle;
	}

	public virtual void Settle ()
	{
		if (m_collider != null) {
			m_collider.isTrigger = false;
		}
		if (m_rigidbody != null) {
			m_rigidbody.isKinematic = false;
		}

		M_Event.FireObjectEvent (MObjectEvent.UnitSettled, new ObjArg (this));
	}

	public virtual void Delete()
	{
		gameObject.SetActive (false);
		m_stateMachine.State = State.Deleted;
	}

	override protected void MAwake ()
	{
		if (m_collider == null)
			m_collider = GetComponent<Collider> ();
		if (m_rigidbody == null)
			m_rigidbody = GetComponent<Rigidbody> ();

		InitStateMachine ();
	}

	override protected void MUpdate ()
	{
		m_stateMachine.Update ();
	}

}
