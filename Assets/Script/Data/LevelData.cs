using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;


/// <summary>
/// Scritable object for saving the level data
/// </summary> 
[CreateAssetMenu(fileName="LevelData",menuName="DoMiNo/Level",order = 1)]
public class LevelData : ScriptableObject {

	/// <summary>
	/// name of the level
	/// </summary>
	public string name;

	public List<UnitData> unitList;


	public static LevelData SaveData()
	{
		return SaveData("Default");
	}

	public static LevelData SaveData( string levelName )
	{
		LevelData asset = ScriptableObject.CreateInstance<LevelData>();
		asset.name = levelName;

		asset.unitList = new List<UnitData>();

		foreach(Transform t in Global.world.GetComponentsInChildren<Transform>())
		{
			Unit u = t.GetComponent<Unit>();

			if ( u != null ) {
				UnitData data = UnitData.GenerateData(u);
				asset.unitList.Add(data);
			}
		}
			
		AssetDatabase.CreateAsset(asset, "Assets/Data/" + levelName +  ".asset");
		AssetDatabase.SaveAssets();

		return asset;
	}

	public static void ReadData( string levelName )
	{
		// remove the world 
		GameObject.Destroy( Global.world.gameObject );

		GameObject world = new GameObject("DWorld");
		world.transform.position = Vector3.zero;
		// create a new world
		Transform worldTrans = world.transform;

		UnitPrefabMap map = AssetDatabase.LoadAssetAtPath( "Assets/Data/map.asset" , typeof(UnitPrefabMap)) as UnitPrefabMap;

	
		LevelData level = AssetDatabase.LoadAssetAtPath( "Assets/Data/" + levelName + ".asset" , typeof( LevelData )) as LevelData;

		GameObject[] prefabs = new GameObject[System.Enum.GetNames(typeof(Unit.Type)).Length ];
		foreach( UnitPrefabMap.TypePrefabPair pair in map.map)
		{
			prefabs[(int)pair.type] = pair.prefab;
		}

		foreach( UnitData ud in level.unitList )
		{
			GameObject obj = Instantiate( prefabs[(int)ud.type] ) as GameObject;
			obj.transform.SetParent( worldTrans );
			obj.transform.position = ud.position;
			obj.transform.rotation = ud.rotation;
			obj.transform.localScale = ud.scale;

			Unit u = obj.GetComponent<Unit>();
			u.Init( ud );
		}
	}

	public static void RefreshLevel()
	{
	}
}
