using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

	public InputManager() { s_Instance = this; }
	public static InputManager Instance { get { return s_Instance; } }
	private static InputManager s_Instance;

	delegate void FuncHandler(); 
	Dictionary<KeyCode,FuncHandler> keyBlendDict = new Dictionary<KeyCode, FuncHandler>();

	Vector2 m_ScreenPos;

	void Start()
	{
		keyBlendDict.Add(KeyCode.Space,StartRunning );
		keyBlendDict.Add(KeyCode.Escape,Cancle);

		m_ScreenPos = Global.Vector2Infinity;
	}

	void Update()
	{
		foreach( KeyCode k in keyBlendDict.Keys )
		{
			if ( Input.GetKeyDown( k ) )
			{
				keyBlendDict[k].Invoke();
			}
		}
	}
	void Cancle()
	{
		
		InputArg arg = new InputArg (this);
		arg.type = InputArg.InputType.None;
		M_Event.FireInputCancle (arg);
	}

	void StartRunning()
	{
		MsgArg arg = new MsgArg(this);
		M_Event.FireStartRunning(arg);
	}

	void OnFingerDown(FingerDownEvent e )
	{
		if (Application.isMobilePlatform || Input.GetMouseButtonDown (0)) {
			InputArg arg = new InputArg (this);
			arg.type = InputArg.InputType.Down;
			arg.screenPos = e.Position;
			arg.e = e;

			M_Event.FireInputDown (arg);
		}

	}

	void OnFingerMove( FingerMotionEvent e )
	{
		InputArg arg = new InputArg (this);
		arg.type = InputArg.InputType.None;
		if ( Application.isMobilePlatform ) {
			if (Input.touches.Length > 1)
				arg.type = InputArg.InputType.SubMotion;
			else
				arg.type = InputArg.InputType.Motion;
		} else {
			if (Input.GetMouseButton (1)) // right button
				arg.type = InputArg.InputType.SubMotion;
			else if (Input.GetMouseButton (0)) // left button
				arg.type = InputArg.InputType.Motion;
		}
		arg.screenPos = e.Position;
		arg.offset = e.Finger.DeltaPosition;
		arg.e = e;

		if ( arg.type == InputArg.InputType.Motion )
			M_Event.FireInputMotion (arg);
		if (arg.type == InputArg.InputType.SubMotion)
			M_Event.FireInputSubMotion (arg);	

		if (e.Phase == FingerMotionPhase.Updated)
			m_ScreenPos = e.Position;
		else if (e.Phase == FingerMotionPhase.Ended)
			m_ScreenPos = Global.Vector2Infinity;
	}

	void OnTwist( TwistGesture g )
	{
		InputArg arg = new InputArg (this);
		arg.type = InputArg.InputType.Twist;
		arg.delta = g.DeltaRotation;
		arg.g = g;

		M_Event.FireInputTwist (arg);
		
	}

	void OnPinch( PinchGesture g )
	{
		Debug.Log ("OnPinch");
		InputArg arg = new InputArg (this);
		arg.type = InputArg.InputType.Pinch;
		arg.delta = g.Delta;
		arg.g = g;

		M_Event.FireInputPinch (arg);
	}

	public Vector2 GetScreenPosition()
	{
		if (Application.isMobilePlatform)
			return m_ScreenPos;
	

		return Input.mousePosition;
	}

}