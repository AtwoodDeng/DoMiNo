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

	/// <summary>
	/// A parameter of vector3
	/// </summary>
	public Vector3 V3Para;


	/// <summary>
	/// Generates the basic data of an unit.
	/// </summary>
	/// <returns>The data.</returns>
	/// <param name="u">U.</param>
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
