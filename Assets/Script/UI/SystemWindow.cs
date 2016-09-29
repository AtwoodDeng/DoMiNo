using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

public class SystemWindow : MWindow
{

	[SerializeField] Text levelName;

	public void Save ()
	{
		string level = "";
		if (levelName != null)
			level = levelName.text;
		if (level == "")
			level = "Default";
		LevelData.SaveData ( level );
	}

	public void Load ()
	{
		string level = "";
		if (levelName != null)
			level = levelName.text;
		if (level == "")
			level = "Default";

		LevelData.ReadData (level);
	}

	public void Undo ()
	{
		Debug.Log ("UnDo");
		InputArg arg = new InputArg (this);

		M_Event.FireInput (MInputType.Undo, arg);
	}

	public void Redo()
	{
		InputArg arg = new InputArg (this);

		M_Event.FireInput (MInputType.Redo, arg);
	}
}
