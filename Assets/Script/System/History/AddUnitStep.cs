using UnityEngine;
using System.Collections;

public class AddUnitStep : IHistoryStep {

	EditableUnit unit;

	public HistoryStepType GetType()
	{
		return HistoryStepType.AddUnit;
	}

	public void Record()
	{
		unit = HistoryManager.Instance.editableUnitSave;
	}

	public void Undo()
	{
		if ( unit != null )
		unit.gameObject.SetActive (false);
	}

	public void Redo()
	{
		if (unit != null) {
			
			unit.gameObject.SetActive (true);

			unit.Settle ();
		}
	}

	public void Clean()
	{
		if (unit != null) {
			GameObject.Destroy (unit.gameObject);
		}
	}
}
