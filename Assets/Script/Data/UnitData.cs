using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Unit data is scriptable object to record the information of the single object
/// </summary>

[System.Serializable]
public class UnitData : System.Object {


	/// <summary>
	/// The type of the unit
	/// </summary>
	public Unit.Type type;

	/// <summary>
	/// an unique id of the unit
	/// </summary>
	public int unitID;

	/// <summary>
	/// The position of the unit in the world
	/// </summary>
	public Vector3 position;

	/// <summary>
	/// The rotation of the unit in the world
	/// </summary>
	public Quaternion rotation;

	/// <summary>
	/// The scale of the unit
	/// </summary>
	public Vector3 scale;

	static public UnitData GenerateData( Unit u )
	{
		UnitData data = new UnitData();

		data.type = u.type;
		data.position  = u.transform.position;
		data.rotation = u.transform.rotation;
		data.scale = u.transform.localScale;
		data.unitID = u.unitID;

		return data;
	}
}
