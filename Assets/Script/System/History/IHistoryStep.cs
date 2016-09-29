using UnityEngine;
using System.Collections;

 interface IHistoryStep {

	HistoryStepType GetType();

	void Record();

	void Undo();

	void Redo ();

	void Clean();
}

public enum HistoryStepType
{
	None,
	AddUnit,
}
