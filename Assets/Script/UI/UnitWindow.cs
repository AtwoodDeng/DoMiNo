using UnityEngine;
using System.Collections;

public class UnitWindow : MonoBehaviour {

	public UnitWindow() { s_Instance = this; }
	public static UnitWindow Instance { get { return s_Instance; } }
	private static UnitWindow s_Instance;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	GameObject lastUnitPrefab;

	public void AddLastUnit()
	{
		OnAddUnit (lastUnitPrefab);
	}

	public void OnAddUnit( GameObject unitPrefab )
	{
		lastUnitPrefab = unitPrefab;
		MsgArg msg = new MsgArg (this);
		M_Event.FireStartAdding (msg);

		GameObject obj = Instantiate (unitPrefab) as GameObject;

		Unit unit = obj.GetComponent<Unit> ();
		unit.StartEditting ();
	}
}
