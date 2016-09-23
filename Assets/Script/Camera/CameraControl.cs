using UnityEngine;
using System.Collections;

public class CameraControl : MBehavior {

	public CameraControl() { s_Instance = this; }
	public static CameraControl Instance { get { return s_Instance; } }
	private static CameraControl s_Instance;

	protected override void MAwake ()
	{
		m_targetPosition = Camera.main.transform.position;
		m_targetRotateCenter = m_rotateCenter = Vector3.zero;
		camTrans = Camera.main.transform;
	}

	Transform camTrans;

	/// <summary>
	/// The rate the camera move close to the target persecond
	/// </summary>
	[SerializeField] float closeRate = 0.8f;

	/// <summary>
	/// The sphere radius for camera rotation
	/// </summary>
	Vector3 m_rotateCenter;
	Vector3 m_targetRotateCenter;

//	private Quaternion m_targetRotation;
//	public Quaternion TargetRotation
//	{
//		get {
//			return m_targetRotation;
//		}
//	}

	private Vector3 m_targetPosition;
	public Vector3 TargetPosition
	{
		get {
			return m_targetPosition;
		}
	}

	/// <summary>
	/// Moves the camera position according to the screen offset.
	/// </summary>
	/// <param name="screenOffset">Screen offset.</param>
	/// <param name="isForce">If set to <c>true</c> is force.</param>
	public void MoveCameraPositionByScreenOffset( Vector2 screenOffset , bool isForce = false )
	{
		Vector3 offset = - screenOffset.x * camTrans.right - screenOffset.y * camTrans.up;
		SetCameraPosition (offset, true, isForce);
	}

	/// <summary>
	/// set the position of the camera
	/// </summary>
	/// <param name="position">Position.</param>
	/// <param name="isRelative">If set to <c>true</c> is relative.</param>
	/// <param name="isForce">If set to <c>true</c> is force to set the position immediately.</param>
	public void SetCameraPosition( Vector3 position , bool isRelative = false , bool isForce = false )
	{
		Vector3 deltaPos = position - camTrans.position ;
		if (isRelative)
			deltaPos = position;
		
		m_targetPosition += deltaPos;
		m_targetRotateCenter += deltaPos;

		if (isForce) {
			camTrans.position = m_targetPosition;
			m_rotateCenter = m_targetRotateCenter;
		}
	}


	/// <summary>
	/// Rotates the camera by screen offset.
	/// </summary>
	/// <param name="screenOffset">Screen offset.</param>
	/// <param name="isForce">If set to <c>true</c> is force to rotate immediately.</param>
	public void RotateCameraByScreenOffset( Vector2 screenOffset , bool isForce = false )
	{
		float radius = (m_targetPosition - m_targetRotateCenter).magnitude;
		Vector3 camToPosDelta = m_targetPosition - screenOffset.x * camTrans.right - screenOffset.y * camTrans.up - m_targetRotateCenter;
		m_targetPosition = m_targetRotateCenter + camToPosDelta.normalized * radius;

		if (isForce) {
			camTrans.position = m_targetPosition;
		}
	}

	/// <summary>
	/// Changes the field of view of the camera.
	/// </summary>
	/// <param name="deltaSize">Delta size.</param>
	public void ChangeFieldOfView( float deltaSize)
	{
		Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView * ( 1f + deltaSize) ,  20f , 100f );

	}

	protected override void MUpdate ()
	{
		camTrans.position = Vector3.Lerp ( camTrans.position, m_targetPosition, closeRate * Time.deltaTime);
		m_rotateCenter = Vector3.Lerp (m_rotateCenter, m_targetRotateCenter, closeRate * Time.deltaTime);
//		camTrans.rotation = Quaternion.Lerp (camTrans.rotation, TargetRotation, closeRate * Time.deltaTime);
		camTrans.LookAt( m_rotateCenter );
	}

}
