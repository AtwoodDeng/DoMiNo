using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// map the prefab with the unit type
/// </summary>
[CreateAssetMenu(fileName="Assets/Data/map",menuName="DoMiNo/map",order = 1)]
public class UnitPrefabMap : ScriptableObject {

	[System.Serializable]
	public class TypePrefabPair
	{
		public Unit.Type type;
		public GameObject prefab;
	}
	public List<TypePrefabPair> map;

}
