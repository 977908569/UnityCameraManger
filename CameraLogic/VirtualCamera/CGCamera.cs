using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGCamera:VirtualCameraBase
{
	public Transform cgTarget;
	public float scale = 1;
	private void InputUpdate()
	{
		if (cgTarget != null)
		{
			transform.position = cgTarget.transform.position * scale;
			transform.rotation = cgTarget.transform.rotation * Quaternion.Euler(0, 180, 0);
		}
	}

	public override void UpdateCameraInfo(Vector3 worldUp)
	{
		base.UpdateCameraInfo(worldUp);
		InputUpdate();
	}

	public override void OnTransitionFromCamera(VirtualCameraBase fromCam, Vector3 worldUp)
	{
		base.OnTransitionFromCamera(fromCam, worldUp);
	}
}
