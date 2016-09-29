using UnityEngine;
using System.Collections;
using System;

public class Unit : MBehavior {

	static int  ID_seed = 0 ;
	private int m_unitID;
	public int unitID { get { return m_unitID; } }

	protected override void MAwake ()
	{
		base.MAwake ();
		m_unitID = ID_seed++;
	}

	public enum Type
	{
		None,
		BasicDomino,
		BasicBall,
		FixedBox,
		Starter,
		Playground,
	}
	public Type type;

	public virtual void Init( UnitData data ) {
		type = data.type;
		m_unitID = data.unitID;
	}

	public virtual UnitData ToData()
	{
		return UnitData.GenerateData (this);
	}

}
