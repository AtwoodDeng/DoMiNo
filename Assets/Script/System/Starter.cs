﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Starter : MonoBehaviour {
	[SerializeField] Vector3 pushDirection;
	[SerializeField] float pushDuration = 0.5f;

	void OnEnable()
	{
		M_Event.StartRunning += M_Event_StartRunning;
	}

	void M_Event_StartRunning (MsgArg arg)
	{
		Sequence seq = DOTween.Sequence ();
		seq.Append (transform.DOMove (pushDirection, pushDuration).SetRelative (true).SetEase (Ease.InBack));
		seq.Append (transform.DOMove (transform.position, pushDuration));
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color (1f, 0.5f, 0);
		Gizmos.DrawLine (transform.position, transform.position + pushDirection);

	}
}
