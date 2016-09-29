using UnityEngine;
using System.Collections;

public class InputManager : MBehavior {

	public InputManager() { s_Instance = this; }
	public static InputManager Instance { get { return s_Instance; } }
	private static InputManager s_Instance;

	protected override void MAwake ()
	{
		base.MAwake ();
		if (s_Instance == null)
			s_Instance = this;
	}

	/// <summary>
	/// Gets the position of focus point on the screen .
	/// </summary>
	/// <returns><c>true</c>, if screen position was gotten, <c>false</c> otherwise.</returns>
	/// <param name="screenPos">the position of the screen point.</param>
	public virtual bool GetScreenPos( out Vector2 screenPos )
	{
		screenPos = Vector2.zero;
		return false;
	}

	/// <summary>
	/// Sends the redo event .
	/// </summary>
	protected void SendRedo()
	{
		InputArg arg = new InputArg (this);
		arg.type = MInputType.Redo;
		M_Event.FireInput (arg);
	}

	/// <summary>
	/// Sends the undo event .
	/// </summary>
	protected void SendUndo()
	{
		InputArg arg = new InputArg (this);
		arg.type = MInputType.Undo;
		M_Event.FireInput (arg);
	}

	/// <summary>
	/// Sends the cancle event .
	/// </summary>
	protected void SendCancle()
	{
		InputArg arg = new InputArg (this);
		arg.type = MInputType.Cancle;
		M_Event.FireInput (arg);
	}

	/// <summary>
	/// Sends the start running event.
	/// </summary>
	protected void SendStartRunning()
	{
		InputArg arg = new InputArg (this);
		arg.type = MInputType.Start;
		M_Event.FireInput (arg);
	}

	/// <summary>
	/// Sends down event.
	/// </summary>
	/// <param name="e">E the down event </param>
	protected void SendDown( Vector2 pos )
	{
		InputArg arg = new InputArg (this);
		arg.type = MInputType.MainButtonDown;
		arg.screenPos = pos;

		M_Event.FireInput ( arg);
	}

	/// <summary>
	/// Sends up event.
	/// </summary>
	/// <param name="pos">Position.</param>
	protected void SendUp( Vector2 pos )
	{
		InputArg arg = new InputArg (this);
		arg.type = MInputType.MainButtonUp;
		arg.screenPos = pos;

		M_Event.FireInput ( arg);
	}

	/// <summary>
	/// Sends the motion event.
	/// </summary>
	/// <param name="e">E.</param>
	protected void SendMotion( Vector2 pos , Vector2 offset , InputArg.State state )
	{
		InputArg arg = new InputArg (this);
		arg.type = MInputType.Motion;
		arg.screenPos = pos;
		arg.offset = offset;
		arg.state = state;

		M_Event.FireInput (  arg);
	}

	/// <summary>
	/// Sends the sub motion event.
	/// </summary>
	/// <param name="e">E.</param>
	protected void SendSpin( Vector2 pos , Vector2 offset , InputArg.State state )
	{
		InputArg arg = new InputArg (this);
		arg.type = MInputType.Spin;
		arg.screenPos = pos;
		arg.offset = offset;


		M_Event.FireInput ( arg);
	}

	/// <summary>
	/// Sends the spin event.
	/// </summary>
	protected void SendZoom( float delta )
	{
		InputArg arg = new InputArg (this);
		arg.type = MInputType.Zoom;
		arg.delta = delta;

		M_Event.FireInput (arg);
	}


	/// <summary>
	/// (Not recommend) Send a input event through a custom arg
	/// </summary>
	/// <param name="arg">Argument.</param>
	protected void SendEvent( InputArg arg )
	{
		M_Event.FireInput (arg);
	}
}
