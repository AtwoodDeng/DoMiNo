using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HistoryManager : MBehavior
{

	public static HistoryManager Instance { get { return s_Instance; } }

	private static HistoryManager s_Instance;

	private Stack<IHistoryStep> historyStepStack;
	private Stack<IHistoryStep> undoStepStack;

	public EditableUnit editableUnitSave;

	protected override void MAwake ()
	{
		base.MAwake ();

		if (s_Instance == null)
			s_Instance = this;

		historyStepStack = new Stack<IHistoryStep> ();
		undoStepStack = new Stack<IHistoryStep> ();
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();

		M_Event.objectEvents [(int)MObjectEvent.UnitSettled] += OnUnitSettled;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();

		M_Event.objectEvents [(int)MObjectEvent.UnitSettled] -= OnUnitSettled;
	}

	void OnUnitSettled (ObjArg arg)
	{
		EditableUnit unit = (EditableUnit)arg.sender;

		editableUnitSave = unit;

		var step = new AddUnitStep ();

		step.Record ();

		historyStepStack.Push (step);

	}

	public void CleanUp ()
	{
		while (undoStepStack.Count > 0) {
			var step = undoStepStack.Pop ();
			step.Clean ();
		}
	}

	public void Undo ()
	{
		Debug.Log ("Undo");
		if (historyStepStack.Count > 0) {
			var step = historyStepStack.Pop ();

			step.Undo ();

			undoStepStack.Push (step);
		}
	}

	public void Redo ()
	{
		if (undoStepStack.Count > 0) {
			var step = undoStepStack.Pop ();
			step.Redo ();

		}

	}

}
