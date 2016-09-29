using UnityEngine;
using System.Collections;

public class UnitWindow : MWindow {

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

		GameObject obj = Instantiate (unitPrefab) as GameObject;
		obj.transform.rotation = lastUnitRotation;

		EditableUnit unit = obj.GetComponent<EditableUnit> ();

		WindowArg msg = new WindowArg (this);
		msg.unit = unit;
	
		M_Event.FireWindowEvent (MWindowEvent.AddUnit, msg);

		unit.StartEditting ();
	}
}
