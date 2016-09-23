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

	public void AddNextUnit( Unit lastUnit )
	{
		if (lastUnit != null)
			OnAddUnit (lastUnitPrefab, lastUnit.transform.rotation);
		else
			OnAddUnit (lastUnitPrefab, Quaternion.identity);
	}

	public void OnAddUnit( GameObject unitPrefab)
	{
		OnAddUnit (unitPrefab, Quaternion.identity); 
	}

	public void OnAddUnit( GameObject unitPrefab , Quaternion lastUnitRotation )
	{
		lastUnitPrefab = unitPrefab;
		MsgArg msg = new MsgArg (this);
		M_Event.FireStartAdding (msg);

		GameObject obj = Instantiate (unitPrefab) as GameObject;
		obj.transform.rotation = lastUnitRotation;

		Unit unit = obj.GetComponent<Unit> ();
		unit.StartEditting ();
	}
}
