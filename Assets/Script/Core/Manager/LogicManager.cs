using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LogicManager : MonoBehaviour {

	public LogicManager() { s_Instance = this; }
	public static LogicManager Instance { get { return s_Instance; } }
	private static LogicManager s_Instance;
		
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
		public float pinchSensity;
	}
	[SerializeField] Sensity sensity;

	void OnEnable()
	{
		M_Event.StartRunning += M_Event_StartRunning;
		M_Event.EndRunning += M_Event_EndRunning;
		M_Event.StartAdding += M_Event_StartAdding;
		M_Event.EditCancle += M_Event_EditCancle;

		M_Event.InputMotion += M_Event_InputMotion;
		M_Event.InputSubMotion += M_Event_InputSubMotion;
		M_Event.InputDown += M_Event_InputDown;
		M_Event.InputPinch += M_Event_InputPinch;
		M_Event.InputTwist += M_Event_InputTwist;
		M_Event.InputCancle += M_Event_InputCancle;

	}

	void M_Event_InputCancle (InputArg arg)
	{
		Debug.Log ("Cancle");
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

	void M_Event_InputSubMotion (InputArg arg)
	{
		switch (mode) {
		case EditMode.Editting:
		case EditMode.Adding:
		case EditMode.Runing:
			// rotate the camera
			CameraControl.Instance.RotateCameraByScreen (arg.offset * sensity.motionSensity , true);
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
			CameraControl.Instance.SetCameraTargetScreen( arg.offset * sensity.subMotionSensity , true  );
			break;
		default:
			break;
		};
	}

	void M_Event_InputTwist (InputArg arg)
	{
		switch (mode) {
		case EditMode.Editting:
			break;
		default:
			break;
		}
	}

	void M_Event_InputPinch (InputArg arg)
	{
		switch (mode) {
		case EditMode.Editting:
		case EditMode.Adding:
		case EditMode.Runing:
			CameraControl.Instance.ChangeSize (arg.delta * sensity.pinchSensity);
			break;
		default:
			break;
		}
	}

	void M_Event_InputDown (InputArg arg)
	{
		switch (mode) {
		case EditMode.Editting:
			break;
		case EditMode.Adding:
			Unit.EdittingUnit.Confirm ();
			UnitWindow.Instance.AddLastUnit ();
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

		M_Event.InputMotion -= M_Event_InputMotion;
		M_Event.InputSubMotion -= M_Event_InputSubMotion;
		M_Event.InputDown -= M_Event_InputDown;
		M_Event.InputPinch -= M_Event_InputPinch;
		M_Event.InputTwist -= M_Event_InputTwist;
		M_Event.InputCancle -= M_Event_InputCancle;
	}


	void OnGUI()
	{
		GUILayout.Label ("Mode " + mode);
	}
}
