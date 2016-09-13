using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LogicManager : MonoBehaviour {

	public LogicManager() { s_Instance = this; }
	public static LogicManager Instance { get { return s_Instance; } }
	private static LogicManager s_Instance;
		
}
