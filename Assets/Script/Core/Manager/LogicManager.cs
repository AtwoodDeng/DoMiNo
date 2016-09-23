using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LogicManager : MonoBehaviour {

	public LogicManager() { s_Instance = this; }
	public static LogicManager Instance { get { return s_Instance; } }
	private static LogicManager s_Instance;

	private InputManager inputManager;
		
	public enum EditMode
	{
		Editting,
		Adding,
		Runing,
		Ending,
	}
	static private EditMode m_mode;
	static public EditMode mode
	{
		get { return m_mode ; }
	}

	[System.Serializable]
	public struct Sensity
	{
		[Range(0,1f)]
		public float motionSensity;
		[Range(0,0.1f)]
		public float subMotionSensity;
		[Range(0,0.02f)]
		public float spinSensity;
	}
	[SerializeField] Sensity sensity;



	void Awake()
	{
		InitManager ();
	}

	void InitManager()
	{
		if (Application.isMobilePlatform) {
			gameObject.AddComponent<InputManagerMobile> ();
		} else {
			gameObject.AddComponent<InputManagerPC> ();
		}
	}

	void OnEnable()
	{
		M_Event.StartRunning += M_Event_StartRunning;
		M_Event.EndRunning += M_Event_EndRunning;
		M_Event.StartAdding += M_Event_StartAdding;
		M_Event.EditCancle += M_Event_EditCancle;
		M_Event.UnitSettled += M_Event_UnitSettled;

		M_Event.inputEvents [(int)MInputType.Motion] += M_Event_InputMotion;
		M_Event.inputEvents [(int)MInputType.Spin] += M_Event_InputSpin;
		M_Event.inputEvents [(int)MInputType.MainButtonDown] += M_Event_InputMainButtonDown;
		M_Event.inputEvents [(int)MInputType.MainButtonUp] += M_Event_InputMainButtonUp;
		M_Event.inputEvents [(int)MInputType.Zoom] += M_Event_InputZoom;
		M_Event.inputEvents [(int)MInputType.Cancle] += M_Event_InputCancle;
		M_Event.inputEvents [(int)MInputType.Start] += M_Event_InputStart;

//		M_Event.InputMotion += M_Event_InputMotion;
//		M_Event.InputSubMotion += M_Event_InputSubMotion;
//		M_Event.InputDown += M_Event_InputDown;
//		M_Event.InputSpin += M_Event_InputSpin;
//		M_Event.InputTwist += M_Event_InputTwist;
//		M_Event.InputCancle += M_Event_InputCancle;

	}

	void M_Event_UnitSettled (MsgArg arg)
	{
		if ( mode == EditMode.Adding) {
			UnitWindow.Instance.AddNextUnit ( (Unit)arg.sender);
		}
	}

	void M_Event_InputMainButtonUp( InputArg arg )
	{
		// do nothing
	}

	void M_Event_InputStart( InputArg arg )
	{
		switch (mode) {
		case EditMode.Editting:
		case EditMode.Adding:
			MsgArg msg = new MsgArg (this);
			M_Event.FireStartRunning (msg );
			break;
		}
	}

	void M_Event_InputCancle (InputArg arg)
	{
		switch (mode) {
		case EditMode.Editting:
			break;
		case EditMode.Adding:
			{
				MsgArg msg = new MsgArg (this);
				M_Event.FireEditCancle (msg);
			}
			break;
		case EditMode.Runing:
			{
				MsgArg msg = new MsgArg (this);
				M_Event.FireEndRunning (msg);
			}
			break;
		default:
			break;
		}
	}

	void M_Event_StartAdding (MsgArg arg)
	{
		m_mode = EditMode.Adding;
	}

	void M_Event_EditCancle (MsgArg arg)
	{
		
		if (m_mode == EditMode.Adding) {
			m_mode = EditMode.Editting;
		}
	}

	void M_Event_InputSpin (InputArg arg)
	{
		switch (mode) {
		case EditMode.Editting:
		case EditMode.Adding:
		case EditMode.Runing:
			// rotate the camera
			CameraControl.Instance.RotateCameraByScreenOffset (arg.offset * sensity.subMotionSensity , true);
			break;
		default:
			break;
		};
		
	}

	void M_Event_InputMotion (InputArg arg)
	{
		switch (mode) {
		case EditMode.Editting:
		case EditMode.Runing:
			// move the camera
			CameraControl.Instance.MoveCameraPositionByScreenOffset( arg.offset * sensity.motionSensity , true  );
			break;
		default:
			break;
		};
	}

	void M_Event_InputZoom (InputArg arg)
	{
		switch (mode) {
		case EditMode.Editting:
		case EditMode.Adding:
		case EditMode.Runing:
			CameraControl.Instance.ChangeFieldOfView (arg.delta * sensity.spinSensity);
			break;
		default:
			break;
		}
	}

	void M_Event_InputMainButtonDown (InputArg arg)
	{
		switch (mode) {
		case EditMode.Editting:
			break;
		case EditMode.Adding:
			break;
		default:
			break;
		}
	}


	void M_Event_EndRunning (MsgArg arg)
	{
		if ( m_mode == EditMode.Runing )
			m_mode = EditMode.Editting;
	}

	void M_Event_StartRunning (MsgArg arg)
	{
		if ( m_mode == EditMode.Editting || m_mode == EditMode.Adding )
			m_mode = EditMode.Runing;
	}

	void OnDisable()
	{
		M_Event.StartRunning -= M_Event_StartRunning;
		M_Event.EndRunning -= M_Event_EndRunning;
		M_Event.StartAdding -= M_Event_StartAdding;
		M_Event.EditCancle -= M_Event_EditCancle;

		M_Event.inputEvents [(int)MInputType.Motion] -= M_Event_InputMotion;
		M_Event.inputEvents [(int)MInputType.Spin] -= M_Event_InputSpin;
		M_Event.inputEvents [(int)MInputType.MainButtonDown] -= M_Event_InputMainButtonDown;
		M_Event.inputEvents [(int)MInputType.Zoom] -= M_Event_InputZoom;
		M_Event.inputEvents [(int)MInputType.Cancle] -= M_Event_InputCancle;
		M_Event.inputEvents [(int)MInputType.Start] -= M_Event_InputStart;

//		M_Event.InputMotion -= M_Event_InputMotion;
//		M_Event.InputSubMotion -= M_Event_InputSubMotion;
//		M_Event.InputDown -= M_Event_InputDown;
//		M_Event.InputSpin -= M_Event_InputSpin;
//		M_Event.InputTwist -= M_Event_InputTwist;
//		M_Event.InputCancle -= M_Event_InputCancle;
	}


	void OnGUI()
	{
		GUILayout.Label ("Mode " + mode);
	}
}
