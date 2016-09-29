using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManagerPC : InputManager {


	delegate void FuncHandler(); 
	Dictionary<KeyCode,FuncHandler> keyBlendDict = new Dictionary<KeyCode, FuncHandler>();
	Dictionary<KeyCode,FuncHandler> controlKeyBlend = new Dictionary<KeyCode, FuncHandler>();

	protected override void MStart ()
	{
		base.MStart ();
		keyBlendDict.Add(KeyCode.Space,SendStartRunning );
		keyBlendDict.Add(KeyCode.Escape,SendCancle);
		keyBlendDict.Add (KeyCode.Z, SendUndo);
		keyBlendDict.Add (KeyCode.R, SendRedo);
	}

	public override bool GetScreenPos (out Vector2 screenPos)
	{
		// always return the mouse position
		screenPos = Input.mousePosition;
		// always return true
		return true;
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

		foreach (KeyCode k in controlKeyBlend.Keys) {
			if (Input.GetKeyDown (k) && (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl))) {
				controlKeyBlend [k].Invoke ();
			}
		}


	}

	void OnFingerDown(FingerDownEvent e )
	{
		if ( Input.GetMouseButtonDown (0)) {
			SendDown ( e.Position );
		}

	}

	void OnFingerMove( FingerMotionEvent e )
	{
		if (Input.GetMouseButton (1))
			SendSpin (e.Position, e.Finger.DeltaPosition, InputArg.ConvertFromFingerPhase (e.Phase));
		else if (Input.GetMouseButton (0))
			SendMotion (e.Position, e.Finger.DeltaPosition, InputArg.ConvertFromFingerPhase (e.Phase));

	}

	void OnFingerUp( FingerUpEvent e )
	{
		if (Input.GetMouseButtonUp (0)) {
			SendUp (e.Position);
		}
	}

//	void OnTwist( TwistGesture g )
//	{
//		SendTwist (g);
//	}

	void OnPinch( PinchGesture g )
	{
		SendZoom (g.Delta);
	}

}