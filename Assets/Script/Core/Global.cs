﻿using UnityEngine;
using System.Collections;

public class Global {

	static public Vector2 Vector2Infinity = new Vector2(Mathf.Infinity,Mathf.Infinity);
	static public Vector3 Vector3Infinity = new Vector3(99999f , 99999f , 99999f );

	static public int StandableMask = LayerMask.GetMask("Floor" );

	static public Transform world{
		get{
			GameObject world = GameObject.Find ("DWorld");
			if ( world == null )
				world = new GameObject("DWorld");
			return world.transform;
		}
	}

	static public string levelDataPath = "Assets/Data/Level/";
}
