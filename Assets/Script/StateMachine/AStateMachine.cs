using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AStateMachine<T> where T : IFormattable , IConvertible , IComparable {

	private T m_state;
	public bool enable = true;
	public T State
	{
		get {
			return m_state;
		}
		set {
			if ( !value.Equals(m_state) && enable) {
//				Debug.Log ("From " + m_state.ToString () + " to " + value);
				if ( exitState.ContainsKey(m_state) && exitState [m_state] != null )
					exitState [m_state]();
				
				m_state = value;

				if ( enterState.ContainsKey(m_state) && enterState [m_state] != null )
					enterState [m_state]();
			}
		}
	}


//	public Action[] enterState = new Action[System.Enum.GetNames (typeof (T)).Length];
//	public Action[] updateState = new Action[System.Enum.GetNames (typeof (T)).Length];
//	public Action[] exitState = new Action[System.Enum.GetNames (typeof (T)).Length];

	public Dictionary<T,Action> enterState = new Dictionary<T, Action>();
	public Dictionary<T,Action> updateState = new Dictionary<T, Action>();
	public Dictionary<T,Action> exitState = new Dictionary<T, Action>();

	public void AddEnter( T type , Action func )
	{
		if (enterState.ContainsKey (type))
			enterState [type] += func;
		else {
			enterState [type] = func;
		}
	}

	public void AddUpdate( T type , Action func )
	{
		if (updateState.ContainsKey (type))
			updateState [type] += func;
		else {
			updateState [type] = func;
		}
	}

	public void AddExit( T type , Action func )
	{
		if (exitState.ContainsKey (type))
			exitState [type] += func;
		else {
			exitState [type] = func;
		}
	}


	public void Update()
	{
		if (updateState.ContainsKey (State))
			updateState [State] ();
	}
}
