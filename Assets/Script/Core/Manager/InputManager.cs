using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

	public InputManager() { s_Instance = this; }
	public static InputManager Instance { get { return s_Instance; } }
	private static InputManager s_Instance;

	delegate void FuncHandler(); 
	Dictionary<KeyCode,FuncHandler> keyBlendDict = new Dictionary<KeyCode, FuncHandler>();

	void Start()
	{
		keyBlendDict.Add(KeyCode.Space,StartRunning );
		keyBlendDict.Add(KeyCode.Escape,EndRunning);
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

	void StartRunning()
	{
		MsgArg arg = new MsgArg(this);
		M_Event.FireStartRunning(arg);
	}

	void EndRunning()
	{
		MsgArg arg = new MsgArg(this);
		M_Event.FireEndRunning(arg);
	}
}