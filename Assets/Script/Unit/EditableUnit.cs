using UnityEngine;
using System.Collections;

public class EditableUnit : Unit {

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
	}
	public State state
	{
		get {
			return m_stateMachine.State;
		}
	}
	public AStateMachine<State> m_stateMachine;

	private static Unit m_edittingUnit;
	public static Unit EdittingUnit{
		get { return m_edittingUnit; }
	}

	bool isEditting
	{
		get {
			return ( state == State.Turn || state == State.Place) && ( EdittingUnit == this);
		}
	}

	[System.Serializable]
	public struct Sensity
	{
		[Range(0,2)]
		public float rotationSensity;
	}
	[SerializeField] Sensity sensity;


	public Collider m_collider;
	public Rigidbody m_rigidbody;

	[SerializeField] Vector3 initOffset;

	virtual public void OnEnable()
	{
		M_Event.EditCancle += M_Event_EditCancle;
		M_Event.StartRunning += M_Event_StartRunning;

		for (int i = 0; i < System.Enum.GetNames (typeof(MInputType)).Length; ++i) {
			M_Event.inputEvents [i] += OnInput;
		}
	}

	public delegate void InputFeedback( InputArg arg );
	public InputFeedback[] inputFeedback = new InputFeedback[System.Enum.GetNames (typeof (MInputType)).Length];


	void OnInput( InputArg arg )
	{
		if (inputFeedback != null && inputFeedback.Length > ((int)arg.type)) {
			if ( inputFeedback[(int)arg.type] != null )
				inputFeedback [(int)arg.type] (arg);
		}
	}

	/// ===================  Start State Machine  ========================
	/// <summary>
	/// Initilize the state machine.
	/// </summary>

	virtual public void InitStateMachine()
	{
		m_stateMachine = new AStateMachine<State>();
		m_stateMachine.State = State.None;
		m_stateMachine.AddEnter(State.Place , EnterPlacing );
		m_stateMachine.AddUpdate(State.Place , UpdatePlacing );
		m_stateMachine.AddExit(State.Place , ExitPlacing );

		m_stateMachine.AddEnter(State.Turn , EnterSpinning );
		m_stateMachine.AddUpdate(State.Turn , UpdateSpinning );
		m_stateMachine.AddExit(State.Turn , ExitSpinning );

		m_stateMachine.AddEnter(State.Settle , EnterSettled );
		m_stateMachine.AddUpdate(State.Settle , UpdateSettled );
		m_stateMachine.AddExit(State.Settle , ExitSettled );
	}

	virtual protected void EnterPlacing()
	{
		m_edittingUnit = this;
		if (m_collider != null)
			m_collider.isTrigger = true;
		if (m_rigidbody != null)
			m_rigidbody.isKinematic = true;
		transform.SetParent (Global.world);

		inputFeedback [(int)MInputType.MainButtonDown] = delegate(InputArg arg) {
			m_stateMachine.State = State.Turn;
		};
	}

	virtual protected void UpdatePlacing()
	{
		if (EdittingUnit == this) {
			Vector2 screenPosition;
			if (InputManager.Instance.GetScreenPos ( out screenPosition)) {
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

	virtual protected void ExitPlacing()
	{
		inputFeedback [(int)MInputType.MainButtonDown] = null;
	}

	virtual protected void EnterSpinning()
	{
		inputFeedback [(int)MInputType.MainButtonUp] = delegate(InputArg arg) {
			m_stateMachine.State = State.Settle;
		};

		inputFeedback [(int)MInputType.Motion] = delegate (InputArg arg) {
			transform.Rotate( Vector3.up , - sensity.rotationSensity * arg.offset.x );	
		};
	}

	virtual protected void UpdateSpinning()
	{

	}

	virtual protected void ExitSpinning()
	{
		inputFeedback [(int)MInputType.MainButtonUp] = null;

		inputFeedback [(int)MInputType.Motion] = null;
	}

	virtual protected void EnterSettled()
	{
		if (m_collider != null) {
			m_collider.isTrigger = false;
		}
		if (m_rigidbody != null) {
			m_rigidbody.isKinematic = false;
		}

		M_Event.FireUnitSettled (new MsgArg (this));
	}

	virtual protected void UpdateSettled()
	{
	}

	virtual protected void ExitSettled()
	{
	}
		
	/////================== End State Machine ===============================

	void M_Event_StartRunning (MsgArg arg)
	{
	}

	void M_Event_EditCancle (MsgArg arg)
	{
	}

	public virtual void Confirm()
	{}

	virtual public void OnDisable()
	{
		M_Event.StartRunning -= M_Event_StartRunning;
		M_Event.EditCancle -= M_Event_EditCancle;

		for (int i = 0; i < System.Enum.GetNames (typeof(MInputType)).Length; ++i) {
			M_Event.inputEvents [i] -= OnInput;
		}
	}

	public virtual void StartEditting()
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

	override public void Init( UnitData data )
	{
		base.Init(data);
		m_stateMachine.State = State.Settle;
	}

	public virtual void Place ()
	{
		if ( state == State.Place )
		{

		}
	}

	override protected void MAwake()
	{
		if (m_collider == null)
			m_collider = GetComponent<Collider> ();
		if (m_rigidbody == null)
			m_rigidbody = GetComponent<Rigidbody> ();

		InitStateMachine ();
	}

	override protected void MUpdate()
	{
		m_stateMachine.Update ();
	}

}
