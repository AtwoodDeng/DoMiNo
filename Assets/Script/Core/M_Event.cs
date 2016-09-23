using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public enum MInputType
{
	None,
	Motion,
	Spin,
	MainButtonDown,
	MainButtonUp,
	Zoom,
	Cancle,
	Start,
}

public enum MEditorEvent
{
	None,
	StartRunning,
	EndRunning,
	StartAdding,
	EditCancle,
	UnitSettled,
}

public class M_Event : MonoBehaviour {

	/// <summary>
	/// Event handler. handle the event with basic arg
	/// </summary>
	public delegate void EventHandler(BasicArg arg);

	public static event EventHandler StartApp;
	public static void FireStartApp(BasicArg arg){if ( StartApp != null ) StartApp(arg) ; }

	/// <summary>
	/// Message events
	/// </summary>
	public delegate void MsgHandler(MsgArg arg);

	public static event MsgHandler StartRunning;
	public static void FireStartRunning(MsgArg arg) { if ( StartRunning != null ) StartRunning(arg); }

	public static event MsgHandler EndRunning;
	public static void FireEndRunning(MsgArg arg) { if ( EndRunning != null ) EndRunning(arg); }

	public static event MsgHandler StartAdding;
	public static void FireStartAdding(MsgArg arg) { if ( StartAdding != null ) StartAdding(arg); }

	public static event MsgHandler EditCancle;
	public static void FireEditCancle(MsgArg  arg) { if ( EditCancle != null ) EditCancle(arg); }

	public static event MsgHandler UnitSettled;
	public static void FireUnitSettled(MsgArg  arg) { if ( UnitSettled != null ) UnitSettled(arg); }

	/// <summary>
	/// Editor events
	/// </summary>
	public delegate void EditorHandler(EditorArg arg );
	public static EditorHandler[] editorEvents = new EditorHandler[System.Enum.GetNames (typeof(MEditorEvent)).Length];
	public static void FireEditorEvent (EditorArg arg)
	{
		if ( arg.type != MEditorEvent.None ) {
			FireEditorEvent (arg.type, arg);
		}
	}
	public static void FireEditorEvent( MEditorEvent type , EditorArg arg )
	{
		if (editorEvents [(int)type] != null) {
			arg.type = type;
			editorEvents [(int)type] (arg);
		}
	}

	/// <summary>
	/// Input events
	/// </summary>
	public delegate void InputHandler( InputArg arg );

	public static InputHandler[] inputEvents = new InputHandler[System.Enum.GetNames (typeof (MInputType)).Length];
	public static void FireInput( InputArg arg )
	{
		if (arg.type != MInputType.None) {
			FireInput (arg.type, arg);
		}
	}
	public static void FireInput( MInputType type , InputArg arg )
	{
		if ( inputEvents[(int)type] != null )
		{
			arg.type = type;
			inputEvents [(int)type] ( arg );
		}
	
	}

//	public static event InputHandler InputMotion;
//	public static void FireInputMotion(InputArg arg) { if (InputMotion != null) InputMotion (arg); }
//
//	public static event InputHandler InputSubMotion;
//	public static void FireInputSubMotion( InputArg arg) { if (InputSubMotion != null) InputSubMotion (arg); }
//
//	public static event InputHandler InputDown;
//	public static void FireInputDown(InputArg arg) { if (InputDown != null) InputDown (arg); }
//
//	public static event InputHandler InputSpin;
//	public static void FireInputSpin(InputArg arg) { if (InputSpin != null) InputSpin (arg); }
//
//	public static event InputHandler InputTwist;
//	public static void FireInputTwist(InputArg arg) { if (InputTwist != null) InputTwist (arg); }
//
//	public static event InputHandler InputCancle;
//	public static void FireInputCancle(InputArg arg) {
//		if (InputCancle != null)
//			InputCancle (arg);
//	}
}

public class BasicArg : EventArgs
{
	public BasicArg(object _this){m_sender = _this;}
	object m_sender;
	public object sender{get{return m_sender;}}
}

public class MsgArg : BasicArg
{
	public MsgArg(object _this):base(_this){}
	Dictionary<string,object> m_dict;
	Dictionary<string,object> dict
	{
		get {
			if ( m_dict == null )
				m_dict = new Dictionary<string, object>();
			return m_dict;
		}
	}
	public void AddMessage(string key, object val)
	{
		dict.Add(key, val);
	}
	public object GetMessage(string key)
	{
		object res;
		dict.TryGetValue(key , out res);
		return res;
	}
	public bool ContainMessage(string key)
	{
		return dict.ContainsKey(key);
	}
}


public class InputArg : BasicArg
{
	public InputArg(object _this):base(_this){}

	/// <summary>
	/// The type of the arg.
	/// </summary>
	public MInputType type;
	/// <summary>
	/// offset for motion
	/// </summary>
	public Vector2 offset;
	/// <summary>
	/// Screen position for down
	/// </summary>
	public Vector2 screenPos;
	/// <summary>
	/// delta for pinch (delta distance) & for twist (rotation)
	/// </summary>
	public float delta;

	public enum State
	{
		None,
		Start,
		Update,
		End
	}
	/// <summary>
	/// The state of the input
	/// </summary>
	public State state;


	/// <summary>
	/// Converts from finger phase to the input state
	/// </summary>
	/// <returns>The from finger phase.</returns>
	/// <param name="p">P.</param>
	public static State ConvertFromFingerPhase( FingerMotionPhase p )
	{
		switch (p) {
		case FingerMotionPhase.Started:
			return State.Start;
		case FingerMotionPhase.Updated:
			return State.Update;
		case FingerMotionPhase.Ended:
			return State.End;
		default:
			break;
		}
		return State.None;
	}
	
}

public class EditorArg : MsgArg
{
	public EditorArg(object _this):base(_this){}

	public MEditorEvent type;
}