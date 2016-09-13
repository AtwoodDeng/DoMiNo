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
		Runing,
		Ending,
	}
	static private EditMode m_mode;
	static public EditMode mode
	{
		get { return m_mode ; }
	}

	void OnEnable()
	{
		M_Event.StartRunning += M_Event_StartRunning;
		M_Event.EndRunning += M_Event_EndRunning;
	}

	void M_Event_EndRunning (MsgArg arg)
	{
		m_mode = EditMode.Editting;
	}

	void M_Event_StartRunning (MsgArg arg)
	{
		m_mode = EditMode.Runing;
	}

	void OnDisable()
	{
		M_Event.StartRunning -= M_Event_StartRunning;
		M_Event.EndRunning -= M_Event_EndRunning;
	}



}
