using UnityEngine;
using System.Collections;
using UnityEditor;

public class SystemWindow : MonoBehaviour {

	public void Save()
	{
		LevelData.SaveData();
	}

	public void Load ( string level )
	{
		if ( level == "" )
			level = "Default";

		LevelData.ReadData( level );
	}
}
