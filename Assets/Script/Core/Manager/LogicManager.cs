using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LogicManager : MBehavior
{

	public LogicManager ()
	{
		s_Instance = this;
	}

	public static LogicManager Instance { get { return s_Instance; } }

	private static LogicManager s_Instance;

	private InputManager inputManager;

	private HistoryManager historyManager;

	public enum EditMode
	{
		None,
		Editting,
		Adding,
		Running,
		Ending,
	}

	static private EditMode m_mode;

	static public EditMode Mode {
		get { return Instance.m_stateMachine.State; }
	}

	public AStateMachine<EditMode> m_stateMachine;

	[System.Serializable]
	public struct Sensity
	{
		[Range (0, 0.1f)]
		public float motionSensity;
		[Range (0, 0.1f)]
		public float SpinSensity;
		[Range (0, 0.1f)]
		public float ZoomSensity;
	}

	[SerializeField] Sensity sensity;



	protected override void MAwake ()
	{
		InitManager ();
		InitStateMachine ();
	}

	void InitManager ()
	{
		if (Application.isMobilePlatform) {
			inputManager = gameObject.AddComponent<InputManagerMobile> ();
		} else {
			inputManager = gameObject.AddComponent<InputManagerPC> ();
		}

		historyManager = gameObject.AddComponent<HistoryManager> ();
	}

	void InitStateMachine()
	{
		m_stateMachine = new AStateMachine<EditMode>();

		m_stateMachine.AddEnter (EditMode.Editting, EnterEditting);
		m_stateMachine.AddUpdate (EditMode.Editting, UpdateEditting);
		m_stateMachine.AddExit (EditMode.Editting, ExitEditting);
		m_stateMachine.AddEnter (EditMode.Adding, EnterAdding);
		m_stateMachine.AddUpdate (EditMode.Adding, UpdateAdding);
		m_stateMachine.AddExit (EditMode.Adding, ExitAdding);
		m_stateMachine.AddEnter (EditMode.Running, EnterRunning);
		m_stateMachine.AddUpdate (EditMode.Running, UpdateRunning);
		m_stateMachine.AddExit (EditMode.Running, ExitRunning);
		m_stateMachine.AddEnter (EditMode.Ending, EnterEnding);
		m_stateMachine.AddUpdate (EditMode.Ending, UpdateEnding);
		m_stateMachine.AddExit (EditMode.Ending, ExitEnding);

		m_stateMachine.State = EditMode.Editting;
	}

	void EnterEditting ()
	{
		inputFeedback[(int)MInputType.Start] += delegate(InputArg iarg) {
			m_stateMachine.State = EditMode.Running;
		}; 
		windowFeedback[(int)MWindowEvent.AddUnit] += delegate(WindowArg arg) {
			m_stateMachine.State = EditMode.Adding;
		};

		inputFeedback [(int)MInputType.Undo] += delegate(InputArg arg) {
			HistoryManager.Instance.Undo ();
		};

		inputFeedback [(int)MInputType.Redo] += delegate(InputArg arg) {
			HistoryManager.Instance.Redo ();
		};
	}

	void UpdateEditting ()
	{
	}

	void ExitEditting ()
	{
		inputFeedback[(int)MInputType.Start] = null;
		inputFeedback [(int)MInputType.Undo] = null;
		inputFeedback [(int)MInputType.Redo] = null;
		windowFeedback[(int)MWindowEvent.AddUnit] = null;
	}

	void EnterAdding ()
	{
		EditorArg earg = new EditorArg(this);
		M_Event.FireEditorEvent(MEditorEvent.StartAdding , earg );

		inputFeedback[(int)MInputType.Start] += delegate(InputArg iarg) {
			m_stateMachine.State = EditMode.Running;
		}; 

		inputFeedback[(int)MInputType.Cancle] += delegate(InputArg arg) {
			M_Event.FireEditorEvent(MEditorEvent.EditCancle , new EditorArg(this));
			m_stateMachine.State = EditMode.Editting;
		};

		inputFeedback [(int)MInputType.Undo] += delegate(InputArg arg) {
			HistoryManager.Instance.Undo ();
		};

		inputFeedback [(int)MInputType.Redo] += delegate(InputArg arg) {
			HistoryManager.Instance.Redo ();
		};

		objectFeedback[(int)MObjectEvent.UnitSettled] +=  delegate(ObjArg arg) {
			UnitWindow.Instance.AddNextUnit ((Unit)arg.sender);
		};


	}

	void UpdateAdding ()
	{
	}

	void ExitAdding ()
	{
		inputFeedback[(int)MInputType.Start] = null;
		inputFeedback[(int)MInputType.Cancle] = null;
		inputFeedback [(int)MInputType.Undo] = null;
		inputFeedback [(int)MInputType.Redo] = null;
		objectFeedback[(int)MObjectEvent.UnitSettled] = null;
	}

	void EnterRunning ()
	{
		EditorArg earg = new EditorArg(this);
		M_Event.FireEditorEvent(MEditorEvent.StartRunning , earg );

		LevelData.SaveData("Tem");
		inputFeedback[(int)MInputType.Cancle] += delegate(InputArg iarg) {
			m_stateMachine.State = EditMode.Editting;
		}; 
	}

	void UpdateRunning ()
	{		
	}

	void ExitRunning ()
	{
		EditorArg earg = new EditorArg(this);
		M_Event.FireEditorEvent(MEditorEvent.EndRunning , earg );

		inputFeedback[(int)MInputType.Cancle] = null;
		LevelData.ReadData("Tem");
	}

	void EnterEnding ()
	{
	}

	void UpdateEnding ()
	{
	}

	void ExitEnding ()
	{
	}


	public delegate void InputFeedback (InputArg arg);
	public InputFeedback[] inputFeedback = new InputFeedback[System.Enum.GetNames (typeof(MInputType)).Length];

	public delegate void WindowFeedback (WindowArg arg);
	public WindowFeedback[] windowFeedback = new WindowFeedback[System.Enum.GetNames (typeof(MWindowEvent)).Length];

	public delegate void ObjectFeedback (ObjArg arg);
	public ObjectFeedback[] objectFeedback = new ObjectFeedback[System.Enum.GetNames (typeof(MObjectEvent)).Length];

	protected override void MOnEnable ()
	{
		base.MOnEnable ();

		M_Event.inputEvents [(int)MInputType.Motion] += M_Event_InputMotion;
		M_Event.inputEvents [(int)MInputType.Spin] += M_Event_InputSpin;
		M_Event.inputEvents [(int)MInputType.Zoom] += M_Event_InputZoom;

		for (int i = 0; i < System.Enum.GetNames (typeof(MInputType)).Length; ++i) {
			M_Event.inputEvents [i] += OnInput;
		}
		for (int i = 0; i < System.Enum.GetNames (typeof(MWindowEvent)).Length; ++i) {
			M_Event.windowEvents [i] += OnWindowEvent;
		}
		for (int i = 0; i < System.Enum.GetNames (typeof(MObjectEvent)).Length; ++i) {
			M_Event.objectEvents [i] += OnObjectEvent;
		}
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();

		M_Event.inputEvents [(int)MInputType.Motion] -= M_Event_InputMotion;
		M_Event.inputEvents [(int)MInputType.Spin] -= M_Event_InputSpin;
		M_Event.inputEvents [(int)MInputType.Zoom] -= M_Event_InputZoom;

		for (int i = 0; i < System.Enum.GetNames (typeof(MInputType)).Length; ++i) {
			M_Event.inputEvents [i] -= OnInput;
		}
		for (int i = 0; i < System.Enum.GetNames (typeof(MWindowEvent)).Length; ++i) {
			M_Event.windowEvents [i] -= OnWindowEvent;
		}
		for (int i = 0; i < System.Enum.GetNames (typeof(MObjectEvent)).Length; ++i) {
			M_Event.objectEvents [i] -= OnObjectEvent;
		}
	}


	void OnInput (InputArg arg)
	{
		if (inputFeedback != null && inputFeedback.Length > ((int)arg.type)) {
			if (inputFeedback [(int)arg.type] != null)
				inputFeedback [(int)arg.type] (arg);
		}
	}

	void OnWindowEvent( WindowArg arg )
	{
		if (windowFeedback != null && windowFeedback.Length > ((int)arg.type)) {
			if (windowFeedback [(int)arg.type] != null)
				windowFeedback [(int)arg.type] (arg);
		}
	}

	void OnObjectEvent( ObjArg arg )
	{
		if (objectFeedback != null && objectFeedback.Length > ((int)arg.type)) {
			if (objectFeedback [(int)arg.type] != null)
				objectFeedback [(int)arg.type] (arg);
		}
	}


	protected override void MUpdate ()
	{
		base.MUpdate ();

		m_stateMachine.Update ();
	}

	/// <summary>
	/// for turning the camera
	/// </summary>
	/// <param name="arg">Argument.</param>

	void M_Event_InputSpin (InputArg arg)
	{
		switch (Mode) {
		case EditMode.Editting:
		case EditMode.Adding:
		case EditMode.Running:
			// rotate the camera
			CameraControl.Instance.RotateCameraByScreenOffset (arg.offset * sensity.SpinSensity, true);
			break;
		default:
			break;
		}
		;
		
	}

	/// <summary>
	/// for moving the camera 
	/// </summary>
	/// <param name="arg">Argument.</param>
	void M_Event_InputMotion (InputArg arg)
	{
		switch (Mode) {
		case EditMode.Editting:
		case EditMode.Running:
			// move the camera
			CameraControl.Instance.MoveCameraPositionByScreenOffset (arg.offset * sensity.motionSensity, true);
			break;
		default:
			break;
		}
		;
	}

	/// <summary>
	/// for zooming the camera
	/// </summary>
	/// <param name="arg">Argument.</param>
	void M_Event_InputZoom (InputArg arg)
	{
		switch (Mode) {
		case EditMode.Editting:
		case EditMode.Adding:
		case EditMode.Running:
			CameraControl.Instance.ChangeFieldOfView (arg.delta * sensity.ZoomSensity);
			break;
		default:
			break;
		}
	}
		
	void OnGUI ()
	{
		GUIStyle style = new GUIStyle ();
		style.fontSize = 22;
		style.normal.textColor = Color.black;
		GUILayout.Label ("Mode " + Mode, style);
	}
}
